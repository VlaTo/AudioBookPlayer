using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V4.Media.Session;
using AudioBookPlayer.App.Domain.Models;
using System;
using AndroidX.Core.App;
using AndroidX.Media.Session;
using Xamarin.Forms;
using Application = Android.App.Application;

namespace AudioBookPlayer.App.Android.Services
{
    internal sealed class NotificationService : Java.Lang.Object
    {
        private const string DefaultChannelId = "AUDIOBOOKPLAYER_1";
        private const int DefaultNotificationId = 0x9000;
        private const int ActionStop = -1;
        private const int ActionPrev = 1;
        private const int ActionPlay = 2;
        private const int ActionNext = 3;

        private readonly WeakEventManager eventManager;
        private readonly NotificationManager notificationManager;
        private readonly MediaSessionCompat.Token mediaSessionToken;
        private readonly MediaControllerCompat mediaController;
        private readonly PendingIntent closePendingIntent;
        private NotificationChannel notificationChannel;
        private NotificationCompat.Action previousChapterAction;
        private NotificationCompat.Action playCurrentAction;
        private NotificationCompat.Action nextChapterAction;

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

        public NotificationService(MediaSessionCompat.Token mediaSessionToken)
        {
            this.mediaSessionToken = mediaSessionToken;
            
            eventManager = new WeakEventManager();
            notificationManager = (NotificationManager)Application.Context.GetSystemService(Context.NotificationService);
            mediaController = new MediaControllerCompat(Application.Context, mediaSessionToken);
            closePendingIntent = MediaButtonReceiver.BuildMediaButtonPendingIntent(Application.Context, ActionStop);
        }

        public void ShowInformation(AudioBook audioBook)
        {
            // 1. create media session
            //var componentName = new ComponentName(Application.Context, Class);
            //var mediaSessionCompat = new MediaSessionCompat(Application.Context, "AudioTag1", componentName, pendingIntent);
            //var mediaController = new MediaControllerCompat(Application.Context, mediaSessionToken);
            //var mediaCallback = new MediaCallback(DoPlay, DoPause);
            //var playbackState = new PlaybackStateCompat.Builder()
            //    .SetActions(PlaybackStateCompat.ActionPlayPause)
            //    .Build();
            /*mediaSessionCompat.SetMetadata(
                new MediaMetadataCompat.Builder()
                    .PutString(MediaMetadataCompat.MetadataKeyTitle, "Sample Title")
                    .PutString(MediaMetadataCompat.MetadataKeyAuthor, "Sample Author")
                    .Build()
            );
            mediaSessionCompat.Active = true;
            mediaSessionCompat.SetCallback(mediaCallback);
            mediaSessionCompat.SetFlags((int)MediaSessionFlags.HandlesTransportControls);
            mediaSessionCompat.SetPlaybackState(playbackState);

            var mediaSession = (MediaSession)mediaSessionCompat.MediaSession;*/

            // 2. create notification channel
            /*NotificationChannel notificationChannel = null;

            if (BuildVersionCodes.O <= Build.VERSION.SdkInt)
            {
                var resources = Application.Context.Resources;
                var channelName = resources?.GetString(Resource.String.notification_channel_name);
                var channelDescription = resources?.GetString(Resource.String.notification_channel_description);

                notificationChannel = notificationManager.GetNotificationChannel(DefaultChannelId);

                if (null == notificationChannel)
                {
                    notificationChannel = new NotificationChannel(
                        DefaultChannelId,
                        channelName,
                        NotificationImportance.Default)
                    {
                        Description = channelDescription
                    };

                    notificationManager.CreateNotificationChannel(notificationChannel);
                }
            }*/
            var nc = GetOrCreateNotificationChannel();

            if (null == nc)
            {
                return;
            }

            var notificationBuilder = new NotificationCompat.Builder(Application.Context, nc.Id);

            var playActionIndex = 0;

            if (true)
            {
                notificationBuilder.AddAction(GetOrCreatePreviousChapterAction());
                playActionIndex++;
            }

            if (true)
            {
                notificationBuilder.AddAction(GetOrCreatePlayCurrentAction());
            }

            // 3. create notification
            var mediaStyle = new AndroidX.Media.App.NotificationCompat.MediaStyle()
                .SetMediaSession(mediaSessionToken)
                .SetShowActionsInCompactView(playActionIndex)
                .SetCancelButtonIntent(closePendingIntent)
                .SetShowCancelButton(true);

            //var notification = new NotificationCompat.Builder(Application.Context, nc.Id)
            var notification = notificationBuilder
                .SetStyle(mediaStyle)
                .SetSmallIcon(Resource.Drawable.icon_feed)
                .SetContentTitle("Audio Book Title")
                .SetContentText("Sample Content Text")
                .AddAction(GetOrCreatePlayCurrentAction())
                .SetContentIntent(mediaController.SessionActivity)
                .SetOnlyAlertOnce(true)
                .SetVisibility(NotificationCompat.VisibilityPublic)
                .Build();

            // 4. show/update notification
            notificationManager.Notify(DefaultNotificationId, notification);
        }

        public void HideInformation()
        {
            ;
        }

        private NotificationChannel GetOrCreateNotificationChannel()
        {
            if (null == notificationChannel)
            {
                if (BuildVersionCodes.O >= Build.VERSION.SdkInt)
                {
                    return null;
                }

                notificationChannel = notificationManager.GetNotificationChannel(DefaultChannelId);

                if (null == notificationChannel)
                {
                    var resources = Application.Context.Resources;
                    var channelName = resources?.GetString(Resource.String.notification_channel_name);
                    var channelDescription = resources?.GetString(Resource.String.notification_channel_description);

                    notificationChannel = new NotificationChannel(DefaultChannelId, channelName, NotificationImportance.Low)
                    {
                        Description = channelDescription
                    };

                    notificationManager.CreateNotificationChannel(notificationChannel);
                }
            }

            return notificationChannel;
        }

        private NotificationCompat.Action GetOrCreatePreviousChapterAction()
        {
            if (null == previousChapterAction)
            {
                previousChapterAction = new NotificationCompat.Action(
                    Resource.Drawable.icon_feed,
                    Application.Context.Resources.GetString(Resource.String.notification_action_prev),
                    MediaButtonReceiver.BuildMediaButtonPendingIntent(Application.Context, ActionPrev)
                );
            }

            return previousChapterAction;
        }

        private NotificationCompat.Action GetOrCreatePlayCurrentAction()
        {
            if (null == playCurrentAction)
            {
                playCurrentAction = new NotificationCompat.Action(
                    Resource.Drawable.icon_feed,
                    Application.Context.Resources.GetString(Resource.String.notification_action_play),
                    MediaButtonReceiver.BuildMediaButtonPendingIntent(Application.Context, ActionPlay)
                );
            }

            return playCurrentAction;
        }

        private NotificationCompat.Action GetOrCreateNextChapterAction()
        {
            if (null == nextChapterAction)
            {
                nextChapterAction = new NotificationCompat.Action(
                    Resource.Drawable.icon_feed,
                    Application.Context.Resources.GetString(Resource.String.notification_action_play),
                    MediaButtonReceiver.BuildMediaButtonPendingIntent(Application.Context, ActionPlay)
                );
            }

            return playCurrentAction;
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