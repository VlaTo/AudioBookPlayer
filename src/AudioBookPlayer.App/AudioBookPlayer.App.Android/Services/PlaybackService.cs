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
using System.Linq;

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

        private AudioManager audioManager;
        private NotificationManager notificationManager;
        private MediaPlayer player;
        private AudioBook book;
        private AudioBookChapter currentChapter;
        private PendingIntent pendingIntent;

        //private PlaybackPositionChanged positionChanged;

        public IBinder Binder
        {
            get;
            private set;
        }

        public override void OnCreate()
        {
            var intent = new Intent(Application.Context, typeof(MainActivity));

            base.OnCreate();

            pendingIntent = PendingIntent.GetActivity(Application.Context, 0, intent, PendingIntentFlags.OneShot);
            audioManager = (AudioManager)GetSystemService(Context.AudioService);
            notificationManager = (NotificationManager)GetSystemService(Context.NotificationService);

            // 1. create media session
            //var mediaSessionManager = MediaSessionManager.GetSessionManager(Application.Context);
            var componentName = new ComponentName(Application.Context, Class);
            var mediaSessionCompat = new MediaSessionCompat(Application.Context, "AudioTag1", componentName, pendingIntent);
            var mediaCallback = new MediaCallback(OnPlay, OnPause);
            mediaSessionCompat.SetMetadata(
                new MediaMetadataCompat.Builder()
                    .PutString(MediaMetadataCompat.MetadataKeyTitle, "Sample Title")
                    .PutString(MediaMetadataCompat.MetadataKeyAuthor, "Sample Author")
                    .Build()
            );
            mediaSessionCompat.Active = true;
            mediaSessionCompat.SetCallback(mediaCallback);
            mediaSessionCompat.SetFlags((int) MediaSessionFlags.HandlesTransportControls);

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
            var notification = new Notification.Builder(Application.Context, notificationChannel?.Id)
                .SetStyle(new Notification.MediaStyle().SetMediaSession(mediaSession.SessionToken))
                .SetContentTitle("Sample Title")
                .SetContentText("Sample Content Text")
                .SetContentIntent(pendingIntent)
                .Build();

            // 4. show/update notification
            notificationManager.Notify(DefaultNotificationId, notification);


            book = null;
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
            book = null;

            if (player.IsPlaying)
            {
                player.Stop();
            }

            player = null;
            Binder = null;

            base.OnDestroy();
        }

        /*[return: GeneratedEnum]
        public override StartCommandResult OnStartCommand(
            Intent intent,
            [GeneratedEnum] StartCommandFlags flags,
            int startId)
        {
            switch (intent.Action)
            {
                case IPlaybackService.ActionPlay:
                {
                    var filename = intent.GetStringExtra("Filename");
                    
                    System.Diagnostics.Debug.WriteLine($"[AndroidPlaybackService] [OnStartCommand] Action play -- \"{filename}\"");

                    StartPlayFile(filename);

                    break;
                }
            }

            return StartCommandResult.Sticky;
        }*/

        public void SetBook(AudioBook audioBook)
        {
            book = audioBook;
        }

        public void Play(int chapterIndex, TimeSpan position)
        {
            var contentResolver = Application.Context.ContentResolver;

            currentChapter = book.Chapters[chapterIndex];

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

            // UpdateNotification();

            player.Prepare();

            //var temp = TimeSpan.FromMilliseconds(player.Duration);
            // [0:] [AndroidPlaybackService] [StartPlayFile] Media duration: 32755299 ms
            // [0:] [AndroidPlaybackService] [StartPlayFile] Media duration: 09:05:55.2990000
            //System.Diagnostics.Debug.WriteLine($"[AndroidPlaybackService] [StartPlayFile] Media duration: {temp:c}");

            player.Start();

            //var timer = new Timer(OnTimer, null, TimeSpan.FromSeconds(3.0d), TimeSpan.FromMilliseconds(500.0d));

            /*if (null == player)
            {
                player = new MediaPlayer();
                player.SetAudioAttributes(attributes);
                player.SetWakeMode(ApplicationContext, WakeLockFlags.Partial);

                var notificationId = "abp_notification";
                var notificationName = ApplicationContext.PackageName;
                var channel = new NotificationChannel(notificationId, notificationName, NotificationImportance.Default)
                {
                    Description = "Sample notification description"
                };

                notificationManager.CreateNotificationChannel(channel);

                var intent = new Intent(Application.Context, typeof(MainActivity));
                var pendingIntent = PendingIntent.GetActivity(
                    Application.Context,
                    0,
                    intent,
                    PendingIntentFlags.UpdateCurrent
                );
                var notification = new NotificationCompat.Builder(Application.Context, channel.Id)
                    //.SetStyle(NotificationCompat.BigTextStyle)
                    .SetAutoCancel(false)
                    .SetContentTitle("Sample Title")
                    .SetContentText("Sample content text")
                    .SetSmallIcon(Resource.Drawable.ic_mtrl_chip_close_circle)
                    .SetContentIntent(pendingIntent)
                    .Build();

                notificationManager.Notify(1001, notification);

                //notificationManager.Cancel(1001);

                /--
                var notification = new Notification
                {
                    TickerText = new Java.Lang.String("Started"),
                    Icon = Resource.Drawable.ic_mtrl_chip_close_circle
                };

                notification.Flags |= NotificationFlags.OngoingEvent;
                notification.SetLatestEventInfo(Application.Context, "Sample title", "Sample content", pendingIntent);

                StartForeground(1, notification);
                --/
            }
            else
            {
                player.Reset();
            }*/

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

        /*private Notification CreateNotification()
        {
            return new Notification.Builder(Application.Context, DefaultChannelId)
                .SetStyle(new Notification.MediaStyle().SetMediaSession(player.AudioSessionId))
                .SetContentTitle("Sample Title")
                .SetContentText("Sample Content Text")
                .SetContentIntent(pendingIntent)
                .Build();
        }

        private void CreateNotificationChannel()
        {
            if (BuildVersionCodes.O > Build.VERSION.SdkInt)
            {
                return;
            }

            var channelName = Resources?.GetString(Resource.String.notification_channel_name);
            var channelDescription = Resources?.GetString(Resource.String.notification_channel_description);
            var channel = new NotificationChannel(DefaultChannelId, channelName, NotificationImportance.Default)
            {
                Description = channelDescription
            };

            notificationManager.CreateNotificationChannel(channel);
        }

        private void UpdateNotification()
        {
            var notification = CreateNotification();
            notificationManager.Notify(DefaultNotificationId, notification);
        }*/

        private AudioFocusRequest AcquireAudioFocus()
        {
            var request = new AudioFocusRequestClass.Builder(AudioFocus.GainTransientMayDuck)
                .SetAudioAttributes(CreateAudioAttributes())
                .Build();

            var result = audioManager.RequestAudioFocus(request);

            if (AudioFocusRequest.Granted != result)
            {
                System.Diagnostics.Debug.WriteLine($"[AndroidPlaybackService] [StartPlayFile] Audio focus not accuired!");
            }

            return result;
        }

        private void OnTimer(object state)
        {
            System.Diagnostics.Debug.WriteLine($"[AndroidPlaybackService] [OnTimer] Position: {player.CurrentPosition}");
            //positionChanged.Publish(player.CurrentPosition);
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