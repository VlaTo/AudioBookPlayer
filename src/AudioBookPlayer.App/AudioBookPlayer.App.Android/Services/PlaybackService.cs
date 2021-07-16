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
using System.Threading;
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
        private AudioManager audioManager;
        private NotificationManager notificationManager;
        private MediaPlayer player;
        private AudioBook audioBook;
        private AudioBookChapter currentChapter;
        private PendingIntent pendingIntent;
        private Timer playingTimer;
        private bool isPlaying;

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
            }
        }

        public TimeSpan CurrentPosition
        {
            get;
            private set;
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

        public event EventHandler CurrentChapterChanged
        {
            add => eventManager.AddEventHandler(value);
            remove => eventManager.RemoveEventHandler(value);
        }

        public event EventHandler CurrentPositionChanged
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
        }

        public void Stop()
        {
            if (false == IsPlaying)
            {
                return;
            }

            player.Stop();
            playingTimer.Dispose();
            playingTimer = null;

            DoRaiseIsPlayingChanged();
        }

        public void Play(TimeSpan position)
        {
            var contentResolver = Application.Context.ContentResolver;

            currentChapter = audioBook.Chapters[0];

            var fragment = currentChapter.Fragments[0];
            var source = fragment.SourceFile;
            var uri = global::Android.Net.Uri.Parse(source.ContentUri);
            var descriptor = contentResolver.OpenAssetFileDescriptor(uri, "r");

            player.Reset();
            player.SetDataSource(descriptor);

            if (AudioFocusRequest.Granted != AcquireAudioFocus())
            {
                System.Diagnostics.Debug.WriteLine($"[AndroidPlaybackService] [StartPlayFile] Audio focus not acquired!");
                return;
            }

            player.Prepare();

            if (TimeSpan.Zero < position)
            {
                player.SeekTo((long) position.TotalMilliseconds, MediaPlayerSeekMode.Closest);
            }

            player.Start();

            if (player.IsPlaying != IsPlaying)
            {
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
            CurrentPosition = TimeSpan.FromMilliseconds(player.CurrentPosition);
            DoRaiseCurrentPositionChanged();
        }

        private void DoRaiseIsPlayingChanged()
        {
            eventManager.HandleEvent(this, EventArgs.Empty, nameof(IsPlayingChanged));
        }

        private void DoRaiseAudioBookChanged()
        {
            eventManager.HandleEvent(this, EventArgs.Empty, nameof(AudioBookChanged));
        }

        private void DoRaiseCurrentChapterChanged()
        {
            eventManager.HandleEvent(this, EventArgs.Empty, nameof(CurrentChapterChanged));
        }

        private void DoRaiseCurrentPositionChanged()
        {
            eventManager.HandleEvent(this, EventArgs.Empty, nameof(CurrentPositionChanged));
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
    }
}