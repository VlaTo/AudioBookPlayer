using Android.App;
using Android.Content;
using Android.Media.Session;
using Android.OS;
using Android.Support.V4.Media;
using Android.Support.V4.Media.Session;
using AudioBookPlayer.App.Domain.Models;
using AudioBookPlayer.App.Services;
using System;
using Xamarin.Forms;
using Application = Android.App.Application;

namespace AudioBookPlayer.App.Android.Services
{
    internal sealed class NotificationService : Java.Lang.Object, IRemoteControlService
    {
        private const string DefaultChannelId = "AUDIOBOOKPLAYER_1";
        private const int DefaultNotificationId = 0x9000;

        private readonly WeakEventManager eventManager;
        private NotificationManager notificationManager;
        private PendingIntent pendingIntent;

        public event EventHandler Play
        {
            add => eventManager.AddEventHandler(value);
            remove => eventManager.RemoveEventHandler(value);
        }

        public event EventHandler Pause
        {
            add => eventManager.AddEventHandler(value);
            remove => eventManager.RemoveEventHandler(value);
        }

        public NotificationService()
        {
            eventManager = new WeakEventManager();
        }

        public void ShowInformation(AudioBook audioBook)
        {
            var intent = new Intent(Application.Context, typeof(MainActivity));

            pendingIntent = PendingIntent.GetActivity(Application.Context, 0, intent, PendingIntentFlags.OneShot);
            notificationManager = (NotificationManager) Application.Context.GetSystemService(Context.NotificationService);

            // 1. create media session
            var componentName = new ComponentName(Application.Context, Class);
            var mediaSessionCompat = new MediaSessionCompat(Application.Context, "AudioTag1", componentName, pendingIntent);
            var mediaCallback = new MediaCallback(DoPlay, DoPause);
            var playbackState = new PlaybackStateCompat.Builder()
                .SetActions(PlaybackStateCompat.ActionPlayPause)
                .Build();
            mediaSessionCompat.SetMetadata(
                new MediaMetadataCompat.Builder()
                    .PutString(MediaMetadataCompat.MetadataKeyTitle, "Sample Title")
                    .PutString(MediaMetadataCompat.MetadataKeyAuthor, "Sample Author")
                    .Build()
            );
            mediaSessionCompat.Active = true;
            mediaSessionCompat.SetCallback(mediaCallback);
            mediaSessionCompat.SetFlags((int)MediaSessionFlags.HandlesTransportControls);
            mediaSessionCompat.SetPlaybackState(playbackState);

            var mediaSession = (MediaSession)mediaSessionCompat.MediaSession;

            // 2. create notification channel
            NotificationChannel notificationChannel = null;

            if (BuildVersionCodes.O <= Build.VERSION.SdkInt)
            {
                var resources = Application.Context.Resources;
                var channelName = resources?.GetString(Resource.String.notification_channel_name);
                var channelDescription = resources?.GetString(Resource.String.notification_channel_description);

                notificationChannel = new NotificationChannel(DefaultChannelId, channelName, NotificationImportance.Default)
                {
                    Description = channelDescription
                };

                notificationManager.CreateNotificationChannel(notificationChannel);
            }

            var playAction = new Notification.Action.Builder(null, "Play", pendingIntent).Build();
            // 3. create notification
            var style = new Notification.MediaStyle()
                .SetMediaSession(mediaSession.SessionToken)
                .SetShowActionsInCompactView(0);
            var notification = new Notification.Builder(Application.Context, notificationChannel?.Id)
                .SetStyle(style)
                .SetSmallIcon(Resource.Drawable.icon_feed)
                .SetContentTitle("Audio Book Title")
                .SetContentText("Sample Content Text")
                .SetContentIntent(pendingIntent)
                .AddAction(playAction)
                .Build();

            // 4. show/update notification
            notificationManager.Notify(DefaultNotificationId, notification);
        }

        public void HideInformation()
        {
            ;
        }

        private void DoPlay()
        {
            eventManager.HandleEvent(this, EventArgs.Empty, nameof(Play));
        }

        private void DoPause()
        {
            eventManager.HandleEvent(this, EventArgs.Empty, nameof(Pause));
        }

        private sealed class MediaCallback : MediaSessionCompat.Callback
        {
            private readonly Action onPlay;
            private readonly Action onPause;

            public MediaCallback(Action onPlay, Action onPause)
            {
                this.onPlay = onPlay;
                this.onPause = onPause;
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