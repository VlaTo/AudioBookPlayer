using Android.App;
using Android.Content;
using Android.Media;
using Android.Media.Session;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.Media;
using Android.Support.V4.Media.Session;
using AudioBookPlayer.App.Models;
using System;
using System.Collections.Generic;
using System.Threading;
using Android.Content.Res;
using Xamarin.Forms;
using Application = Android.App.Application;

namespace AudioBookPlayer.App.Android.Services
{
    // https://docs.microsoft.com/en-us/xamarin/android/app-fundamentals/android-audio
    // https://github.com/jamesmontemagno/AndroidStreamingAudio/tree/master/Part%201%20-%20Simple%20Streaming
    // https://docs.microsoft.com/en-us/xamarin/android/app-fundamentals/services/creating-a-service/bound-services
    [Service]
    //[IntentFilter(new []{ IPlaybackService.ActionPlay })]
    public sealed class PlaybackService : Service, IPlaybackService, AudioManager.IOnAudioFocusChangeListener
    {
        private const string DefaultChannelId = "AUDIOBOOKPLAYER_1";
        private const int DefaultNotificationId = 0x9000;

        private readonly WeakEventManager eventManager;
        private readonly Dictionary<string, int> pendingEvents;
        private AudioManager audioManager;
        private NotificationManager notificationManager;
        private MediaPlayer player;
        private AudioBook audioBook;
        private AudioBookChapter currentChapter;
        private int chapterIndex;
        private PendingIntent pendingIntent;
        private Timer playingTimer;
        private bool isPlaying;
        private int updateCount;

        public AudioBook AudioBook
        {
            get => audioBook;
            set
            {
                if (IsPlaying)
                {
                    Stop();
                }

                audioBook = value;
                chapterIndex = -1;
                Position = TimeSpan.Zero;

                RaiseOrPostponeEvent(nameof(AudioBookChanged));
            }
        }

        public TimeSpan Position
        {
            get;
            private set;
        }

        public int ChapterIndex
        {
            get => chapterIndex;
            set
            {
                chapterIndex = value;
                Position = TimeSpan.Zero;

                if (-1 < chapterIndex)
                {
                    currentChapter = audioBook.Chapters[chapterIndex];
                }

                RaiseOrPostponeEvent(nameof(ChapterIndexChanged));
            }
        }

        public bool IsPlaying
        {
            get => isPlaying;
            set
            {
                if (isPlaying != value)
                {
                    isPlaying = value;
                }
            }
        }

        public event EventHandler IsPlayingChanged
        {
            add => eventManager.AddEventHandler(value);
            remove => eventManager.RemoveEventHandler(value);
        }

        public event EventHandler AudioBookChanged
        {
            add => eventManager.AddEventHandler(value);
            remove => eventManager.RemoveEventHandler(value);
        }

        public event EventHandler ChapterIndexChanged
        {
            add => eventManager.AddEventHandler(value);
            remove => eventManager.RemoveEventHandler(value);
        }

        public event EventHandler PositionChanged
        {
            add => eventManager.AddEventHandler(value);
            remove => eventManager.RemoveEventHandler(value);
        }

        public IBinder Binder
        {
            get;
            private set;
        }

        public PlaybackService()
        {
            eventManager = new WeakEventManager();
            pendingEvents = new Dictionary<string, int>();
            updateCount = 0;
        }

        public void Stop()
        {
            if (false == IsPlaying)
            {
                return;
            }

            player.Stop();

            if (null != playingTimer)
            {
                playingTimer.Dispose();
                playingTimer = null;
            }

            DoRaiseIsPlayingChanged();
        }

        public void Play(TimeSpan position)
        {
            if (null == audioBook)
            {
                throw new Exception();
            }

            if (0 > chapterIndex)
            {
                throw new Exception();
            }

            if (audioBook.Chapters.Count <= chapterIndex)
            {
                throw new Exception();
            }

            if (currentChapter.Duration <= position)
            {
                throw new Exception();
            }

            var fragment = GetChapterFragment(position, currentChapter);

            if (null == fragment)
            {
                return;
            }

            var descriptor = OpenFileDescriptor(fragment);

            player.Reset();
            player.SetDataSource(descriptor);

            if (AudioFocusRequest.Granted != AcquireAudioFocus())
            {
                System.Diagnostics.Debug.WriteLine($"[AndroidPlaybackService] [StartPlayFile] Audio focus not acquired!");
                return;
            }

            player.Prepare();

            var pos = player.CurrentPosition;

            var offset = (currentChapter.Start + position).TotalMilliseconds;
            player.SeekTo((long) offset, MediaPlayerSeekMode.Closest);

            player.Start();

            if (player.IsPlaying != IsPlaying)
            {
                IsPlaying = player.IsPlaying;

                DoRaiseIsPlayingChanged();

                if (player.IsPlaying)
                {
                    playingTimer = new Timer(OnTimer, null, TimeSpan.Zero, TimeSpan.FromSeconds(1.0d));
                }
            }
        }

        public override void OnCreate()
        {
            var intent = new Intent(Application.Context, typeof(MainActivity));

            base.OnCreate();

            pendingIntent = PendingIntent.GetActivity(Application.Context, 0, intent, PendingIntentFlags.OneShot);
            audioManager = (AudioManager) GetSystemService(Context.AudioService);
            notificationManager = (NotificationManager) GetSystemService(Context.NotificationService);

            // 1. create media session
            var componentName = new ComponentName(Application.Context, Class);
            var mediaSessionCompat = new MediaSessionCompat(Application.Context, "AudioTag1", componentName, pendingIntent);
            var mediaCallback = new MediaCallback(OnPlay, OnPause);
            var playbackState = new PlaybackStateCompat.Builder()
                .SetActions(PlaybackStateCompat.ActionPlay)
                .Build();
            mediaSessionCompat.SetMetadata(
                new MediaMetadataCompat.Builder()
                    .PutString(MediaMetadataCompat.MetadataKeyTitle, "Sample Title")
                    .PutString(MediaMetadataCompat.MetadataKeyAuthor, "Sample Author")
                    .Build()
            );
            mediaSessionCompat.Active = true;
            mediaSessionCompat.SetCallback(mediaCallback);
            mediaSessionCompat.SetFlags((int) MediaSessionFlags.HandlesTransportControls);
            mediaSessionCompat.SetPlaybackState(playbackState);
            
            var mediaSession = (MediaSession) mediaSessionCompat.MediaSession;

            // 2. create notification channel
            NotificationChannel notificationChannel = null;

            if (BuildVersionCodes.O <= Build.VERSION.SdkInt)
            {
                var channelName = Resources?.GetString(Resource.String.notification_channel_name);
                var channelDescription = Resources?.GetString(Resource.String.notification_channel_description);

                notificationChannel = new NotificationChannel(DefaultChannelId, channelName, NotificationImportance.Default)
                {
                    Description = channelDescription
                };

                notificationManager.CreateNotificationChannel(notificationChannel);
            }
            
            // 3. create notification
            var style = new Notification.MediaStyle()
                .SetMediaSession(mediaSession.SessionToken)
                .SetShowActionsInCompactView();
            var notification = new Notification.Builder(Application.Context, notificationChannel?.Id)
                .SetStyle(style)
                .SetSmallIcon(Resource.Drawable.icon_feed)
                .SetContentTitle("Sample Title")
                .SetContentText("Sample Content Text")
                .SetContentIntent(pendingIntent)
                .Build();

            // 4. show/update notification
            notificationManager.Notify(DefaultNotificationId, notification);
            
            audioBook = null;
            currentChapter = null;
            player = new MediaPlayer();

            player.SetAudioAttributes(CreateAudioAttributes());
            player.SetWakeMode(Application.Context, WakeLockFlags.Partial);
        }

        public override IBinder OnBind(Intent intent)
        {
            Binder = new PlaybackServiceBinder(this);

            return Binder;
        }

        public override bool OnUnbind(Intent intent)
        {
            return base.OnUnbind(intent);
        }

        public override void OnDestroy()
        {
            audioManager = null;
            currentChapter = null;
            audioBook = null;

            if (player.IsPlaying)
            {
                player.Stop();
            }

            player = null;
            Binder = null;

            base.OnDestroy();
        }

        public IDisposable BeginUpdate()
        {
            updateCount++;
            return new UpdateToken(this);
        }
        
        void AudioManager.IOnAudioFocusChangeListener.OnAudioFocusChange([GeneratedEnum] AudioFocus focusChange)
        {
            switch (focusChange)
            {
                case AudioFocus.Gain:
                {
                    System.Diagnostics.Debug.WriteLine($"[AndroidPlaybackService] [OnAudioFocusChange] Start playing");

                    break;
                }

                case AudioFocus.Loss:
                {
                    System.Diagnostics.Debug.WriteLine($"[AndroidPlaybackService] [OnAudioFocusChange] Stop playing");

                    break;
                }

                case AudioFocus.LossTransient:
                {
                    System.Diagnostics.Debug.WriteLine($"[AndroidPlaybackService] [OnAudioFocusChange] Payse playing");

                    break;
                }

                case AudioFocus.LossTransientCanDuck:
                {
                    System.Diagnostics.Debug.WriteLine($"[AndroidPlaybackService] [OnAudioFocusChange] Mute playing");

                    break;
                }
            }
        }

        private void EndUpdate()
        {
            if (0 < updateCount)
            {
                if (0 == --updateCount)
                {
                    RaisePendingEvents();
                }
            }
        }

        private void RaisePendingEvents()
        {
            RaiseAndRemovePendingEvent(nameof(AudioBookChanged));
            RaiseAndRemovePendingEvent(nameof(ChapterIndexChanged));
            RaiseAndRemovePendingEvent(nameof(PositionChanged));
        }

        private void RaiseAndRemovePendingEvent(string eventName)
        {
            if (pendingEvents.TryGetValue(eventName, out var count))
            {
                if (0 == count)
                {
                    return;
                }

                eventManager.HandleEvent(this, EventArgs.Empty, eventName);
                pendingEvents.Remove(eventName);
            }
        }

        private void RaiseOrPostponeEvent(string eventName)
        {
            if (0 == updateCount)
            {
                eventManager.HandleEvent(this, EventArgs.Empty, eventName);
                return;
            }

            if (false == pendingEvents.TryGetValue(eventName, out var count))
            {
                count = 0;
            }

            pendingEvents[eventName] = ++count;
        }

        private static AudioBookChapterFragment GetChapterFragment(TimeSpan position, AudioBookChapter chapter)
        {
            for (var index = 0; index < chapter.Fragments.Count; index++)
            {
                var fragment = chapter.Fragments[index];

                if (fragment.Duration > position)
                {
                    return fragment;
                }

                position -= fragment.Duration;
            }

            return null;
        }

        private static AssetFileDescriptor OpenFileDescriptor(AudioBookChapterFragment fragment)
        {
            var contentResolver = Application.Context.ContentResolver;
            var source = fragment.SourceFile;
            var uri = global::Android.Net.Uri.Parse(source.ContentUri);

            return contentResolver.OpenAssetFileDescriptor(uri, "r");
        }

        private static AudioAttributes CreateAudioAttributes()
            => new AudioAttributes.Builder()
                .SetContentType(AudioContentType.Music)
                .Build();

        private void OnPlay()
        {
            ;
        }

        private void OnPause()
        {
            ;
        }

        private AudioFocusRequest AcquireAudioFocus()
        {
            var request = new AudioFocusRequestClass.Builder(AudioFocus.GainTransientMayDuck)
                .SetAudioAttributes(CreateAudioAttributes())
                .Build();

            var result = audioManager.RequestAudioFocus(request);

            if (AudioFocusRequest.Granted != result)
            {
                System.Diagnostics.Debug.WriteLine($"[AndroidPlaybackService] [StartPlayFile] Audio focus not acquired!");
            }

            return result;
        }

        private void OnTimer(object state)
        {
            Position = TimeSpan.FromMilliseconds(player.CurrentPosition);
            DoRaisePositionChanged();
        }

        private void DoRaiseIsPlayingChanged()
        {
            //eventManager.HandleEvent(this, EventArgs.Empty, nameof(IsPlayingChanged));
            RaiseOrPostponeEvent(nameof(IsPlayingChanged));
        }

        /*private void DoRaiseAudioBookChanged()
        {
            RaiseOrPostponeEvent(nameof(AudioBookChanged));
        }*/

        /*private void DoRaiseChapterIndexChanged()
        {
            //eventManager.HandleEvent(this, EventArgs.Empty, nameof(ChapterIndexChanged));
            RaiseOrPostponeEvent(nameof(ChapterIndexChanged));
        }*/

        private void DoRaisePositionChanged()
        {
            //eventManager.HandleEvent(this, EventArgs.Empty, nameof(PositionChanged));
            RaiseOrPostponeEvent(nameof(PositionChanged));
        }

        /*public async Task PlayAsync(
            System.IO.Stream stream,
            string audioEncoding,
            int sampleRate,
            string audioChannels)
        {
            var player = new MediaPlayer();



            var mediaEncoding = GetAudioEncoding(audioEncoding);// Encoding.Mp3;
            var channels = GetAudioChannels(audioChannels);// ChannelOut.Stereo;
            var audioAttributes = new AudioAttributes.Builder()
                    .SetFlags(AudioFlags.None)
                    .SetContentType(AudioContentType.Music)
                    .SetUsage(AudioUsageKind.Media)
                    .Build();
            var audioFormat = new AudioFormat.Builder()
                .SetEncoding(mediaEncoding)
                .SetSampleRate(sampleRate)
                .SetChannelMask(channels)
                .Build();

            var bufferSize = AudioTrack.GetMinBufferSize(sampleRate, channels, mediaEncoding);

            System.Diagnostics.Debug.WriteLine($"[PlaybackTest] [PlayAsync] Trying to allocate buffer of {bufferSize} bytes");

            using (var audio = new AudioTrack(audioAttributes, audioFormat, bufferSize, AudioTrackMode.Stream, 1))
            {
                audio.SetVolume(1.0f);

                //audio.SetNotificationMarkerPosition((int)streamLength / 2);
                //audio.SetPlaybackPositionUpdateListener(this);
                //audio.SetPositionNotificationPeriod(10);

                audio.Play();

                var buffer = new byte[bufferSize];

                while (true)
                {
                    var count = await stream.ReadAsync(buffer);

                    if (0 == count)
                    {
                        break;
                    }

                    await audio.WriteAsync(buffer, 0, count);
                }

                //audio.SetPositionNotificationPeriod(0);
                //audio.SetPlaybackPositionUpdateListener(null);

                System.Diagnostics.Debug.WriteLine("[PlaybackTest] [PlayAsync] Done playing");

                audio.Release();
            }
        }*/

        /*private static Encoding GetAudioEncoding(string value)
        {
            switch (value)
            {
                case "Encoding.Mp3":
                {
                    return Encoding.Mp3;
                }

                case "Encoding.Aac":
                {
                    return Encoding.AacHeV1;
                }

                case "Encoding.Ac3":
                {
                    return Encoding.Ac3;
                }

                case "Encoding.Pcm16":
                {
                    return Encoding.Pcm16bit;
                }

                case "Encoding.Pcm24":
                {
                    return Encoding.PcmFloat;
                }
            }

            throw new NotSupportedException();
        }

        private static ChannelOut GetAudioChannels(string value)
        {
            switch (value)
            {
                case "ChannelOut.Stereo":
                {
                    return ChannelOut.Stereo;
                }

                case "ChannelOut.Mono":
                {
                    return ChannelOut.Mono;
                }
            }

            throw new NotSupportedException();
        }*/

        private sealed class MediaCallback : MediaSessionCompat.Callback
        {
            public MediaCallback(Action onPlay, Action onPause)
                : base()
            {
            }

            public override void OnPause()
            {
                base.OnPause();
            }

            public override void OnPlay()
            {
                base.OnPlay();
            }
        }

        private sealed class UpdateToken : IDisposable
        {
            private readonly PlaybackService service;
            private bool disposed;

            public UpdateToken(PlaybackService service)
            {
                this.service = service;
            }

            public void Dispose()
            {
                if (disposed)
                {
                    return;
                }

                try
                {
                    service.EndUpdate();
                }
                finally
                {
                    disposed = true;
                }
            }
        }
    }
}