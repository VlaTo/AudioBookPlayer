#nullable enable

using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V4.Media.Session;
using AndroidX.Core.App;
using System;
using Application = Android.App.Application;
using MediaStyle = AndroidX.Media.App.NotificationCompat.MediaStyle;

namespace AudioBookPlayer.App.Android.Services
{
    internal sealed class NotificationService : BroadcastReceiver, NotificationService.IIntents, NotificationService.IChannels
    {
        private const string DefaultChannelId = "AUDIOBOOKPLAYER_1";
        private const int DefaultNotificationId = 0x9000;
        
        public const string IntentActionPlay = "com.libraprogramming.audioplayer.actions.play";
        public const string IntentActionPause = "com.libraprogramming.audioplayer.actions.pause";
        public const string IntentActionSkipToPrev = "com.libraprogramming.audioplayer.actions.skiptoprev";
        public const string IntentActionSkipToNext = "com.libraprogramming.audioplayer.actions.skiptonext";
        public const string IntentActionClose = "com.libraprogramming.audioplayer.actions.close";

        private readonly AudioBooksPlaybackService service;
        private readonly MediaControllerCallback controllerCallback;
        private readonly NotificationManager? notificationManager;
        private readonly PendingIntent? contentIntent;
        private readonly PendingIntent? closePendingIntent;
        private readonly PendingIntent? playIntent;
        private readonly PendingIntent? pauseIntent;
        private readonly PendingIntent? skipToPrevIntent;
        private readonly PendingIntent? skipToNextIntent;
        private MediaControllerCompat? mediaController;
        private MediaControllerCompat.TransportControls? controls;
        private NotificationChannel? notificationChannel;
        private PlaybackStateCompat? playbackState;
        private MediaSessionCompat.Token? sessionToken;
        private int lastPlaybackState;
        private bool shown;

        public MediaSessionCompat.Token? SessionToken
        {
            get => sessionToken;
            set
            {
                if (sessionToken == value)
                {
                    return;
                }

                if (null != sessionToken)
                {
                    if (null != mediaController)
                    {
                        mediaController.UnregisterCallback(controllerCallback);
                    }

                    service.UnregisterReceiver(this);
                }

                sessionToken = value;

                if (null != sessionToken)
                {
                    mediaController = new MediaControllerCompat(Application.Context, sessionToken);
                    controls = mediaController.GetTransportControls();
                }
            }
        }

        public PlaybackStateCompat? PlaybackState
        {
            get => playbackState;
            set
            {
                if (null != playbackState)
                {
                    ;
                }

                playbackState = value;
            }
        }

        public NotificationService(AudioBooksPlaybackService service)
        {
            this.service = service;
            mediaController = null;
            controls = null;
            lastPlaybackState = PlaybackStateCompat.StateNone;
            
            notificationManager = (NotificationManager?)Application.Context.GetSystemService(Context.NotificationService);

            playIntent = CreatePendingIntent(PlaybackStateCompat.ActionPlay, IIntents.Play);
            pauseIntent = CreatePendingIntent(PlaybackStateCompat.ActionPause, IIntents.Pause);
            skipToPrevIntent = CreatePendingIntent(PlaybackStateCompat.ActionSkipToPrevious, IIntents.SkipToPrev);
            skipToNextIntent = CreatePendingIntent(PlaybackStateCompat.ActionSkipToNext, IIntents.SkipToNext);
            closePendingIntent = CreatePendingIntent(0, IIntents.Close);
            contentIntent = CreateContentIntent();

            controllerCallback = new MediaControllerCallback
            {
                OnPlaybackStateChangedImpl = pbs =>
                {
                    playbackState = pbs;

                    if (null != pbs)
                    {
                        if (PlaybackStateCompat.StateStopped == pbs.State || PlaybackStateCompat.StateNone == pbs.State)
                        {
                            HideInformation();
                        }
                        else if (PlaybackStateCompat.StatePlaying == pbs.State || PlaybackStateCompat.StatePaused == pbs.State)
                        {
                            if (shown)
                            {
                                UpdateInformation();
                            }
                            else
                            {
                                ShowInformation();
                            }
                        }
                    }
                },
                OnSessionDestroyedImpl = () =>
                {
                    SessionToken = null;
                }
            };
        }

        public void ShowInformation()
        {
            if (false == shown)
            {
                System.Diagnostics.Debug.WriteLine("[NotificationService] [ShowInformation] Execute");

                SessionToken = service.SessionToken;
                PlaybackState = mediaController?.PlaybackState;
                lastPlaybackState = PlaybackState?.State ?? PlaybackStateCompat.StateNone;

                var notification = CreateNotification();
                
                if (null != notification)
                {
                    var filter = new IntentFilter();

                    filter.AddAction(IIntents.Play);
                    filter.AddAction(IIntents.Pause);
                    filter.AddAction(IIntents.SkipToPrev);
                    filter.AddAction(IIntents.SkipToNext);

                    if (null != mediaController)
                    {
                        mediaController.RegisterCallback(controllerCallback);
                    }

                    service.RegisterReceiver(this, filter);
                    service.StartForeground(IChannels.NotificationID, notification);

                    if (null != notificationManager)
                    {
                        notificationManager.Notify(IChannels.NotificationID, notification);
                    }
                }

                shown = true;
            }
        }

        public void HideInformation()
        {
            if (shown)
            {
                System.Diagnostics.Debug.WriteLine("[NotificationService] [HideInformation] Execute");

                shown = false;

                mediaController?.UnregisterCallback(controllerCallback);

                try
                {
                    if (null != notificationManager)
                    {
                        notificationManager.Cancel(IChannels.NotificationID);
                    }

                    service.UnregisterReceiver(this);
                }
                catch (ArgumentException)
                {
                    // ignore if the receiver is not registered.
                }

                service.StopForeground(true);
            }
        }

        public void UpdateInformation()
        {
            var state = PlaybackState?.State ?? PlaybackStateCompat.StateNone;

            if (shown && state != lastPlaybackState)
            {
                System.Diagnostics.Debug.WriteLine("[NotificationService] [UpdateInformation] Execute");

                var notification = CreateNotification();

                if (null != notification && null != notificationManager)
                {
                    notificationManager.Notify(IChannels.NotificationID, notification);
                }
            }
        }

        public override void OnReceive(Context? context, Intent? intent)
        {
            switch (intent?.Action)
            {
                case IIntents.Pause:
                {
                    controls.Pause();
                    break;
                }

                case IIntents.Play:
                {
                    controls.Play();
                    break;
                }

                case IIntents.SkipToPrev:
                {
                    controls.SkipToPrevious();
                    break;
                }

                case IIntents.SkipToNext:
                {
                    controls.SkipToNext();
                    break;
                }

                default:
                {
                    System.Diagnostics.Debug.WriteLine($"[NotificationService] [OnReceive] Unknown action: \"{intent?.Action}\"");
                    break;
                }
            }
        }

        private void AddSkipToNextAction(NotificationCompat.Builder? notificationBuilder)
        {
            var actions = mediaController?.PlaybackState.Actions ?? 0;
            var canSkipToNext = 0 != (actions & PlaybackStateCompat.ActionSkipToNext);

            if (null != notificationBuilder && canSkipToNext)
            {

                var action = new NotificationCompat.Action(
                    Resource.Drawable.ic_skip_to_next,
                    Application.Context.Resources?.GetString(Resource.String.notification_action_next),
                    skipToNextIntent
                );

                notificationBuilder.AddAction(action);
            }
        }

        private void AddSkipToPrevAction(NotificationCompat.Builder? notificationBuilder, ref int playActionIndex)
        {
            var actions = mediaController?.PlaybackState.Actions ?? 0;
            var canSkipToPrev = 0 != (actions & PlaybackStateCompat.ActionSkipToPrevious);

            if (null != notificationBuilder && canSkipToPrev)
            {
                var action = new NotificationCompat.Action(
                    Resource.Drawable.ic_skip_to_prev,
                    Application.Context.Resources?.GetString(Resource.String.notification_action_prev),
                    skipToPrevIntent
                );

                notificationBuilder.AddAction(action);

                playActionIndex++;
            }
        }

        private void AddPlayPauseAction(NotificationCompat.Builder? notificationBuilder)
        {
            const long mask = PlaybackStateCompat.ActionPlay | PlaybackStateCompat.ActionPlayPause;

            if (null != mediaController)
            {
                var actions = mediaController?.PlaybackState.Actions ?? 0;
                var canPlay = 0 != (actions & mask);

                if (null != notificationBuilder && canPlay)
                {
                    var state = mediaController?.PlaybackState.State ?? PlaybackStateCompat.StateNone;
                    var playing = PlaybackStateCompat.StatePlaying == state;
                    var title = Application.Context.Resources?.GetString(
                        playing
                            ? Resource.String.notification_action_pause
                            : Resource.String.notification_action_play
                    );
                    var action = new NotificationCompat.Action(
                        playing ? Resource.Drawable.ic_pause : Resource.Drawable.ic_play,
                        title,
                        playing ? pauseIntent : playIntent
                    );

                    notificationBuilder.AddAction(action);
                }
            }
        }

        private Notification? CreateNotification()
        {
            var nc = GetOrCreateNotificationChannel();
            var notificationBuilder = new NotificationCompat.Builder(Application.Context, nc?.Id);

            var playActionIndex = 0;

            AddSkipToPrevAction(notificationBuilder, ref playActionIndex);
            AddPlayPauseAction(notificationBuilder);
            AddSkipToNextAction(notificationBuilder);

            notificationBuilder
                .SetStyle(new MediaStyle()
                    .SetMediaSession(SessionToken)
                    .SetShowActionsInCompactView(playActionIndex)
                    .SetCancelButtonIntent(closePendingIntent)
                    .SetShowCancelButton(true))
                .SetSmallIcon(Resource.Drawable.ic_audiobookplayer)
                .SetContentIntent(contentIntent)
                .SetVisibility(NotificationCompat.VisibilityPublic);

            return notificationBuilder.Build();
        }

        private NotificationChannel? GetOrCreateNotificationChannel()
        {
            if (null == notificationChannel)
            {
                if (BuildVersionCodes.O >= Build.VERSION.SdkInt)
                {
                    return null;
                }

                notificationChannel = notificationManager?.GetNotificationChannel(IChannels.ChannelID);

                if (null == notificationChannel)
                {
                    var resources = Application.Context.Resources;
                    var channelName = resources?.GetString(Resource.String.notification_channel_name);
                    var channelDescription = resources?.GetString(Resource.String.notification_channel_description);

                    notificationChannel = new NotificationChannel(IChannels.ChannelID, channelName, NotificationImportance.Low)
                    {
                        Description = channelDescription
                    };

                    if (null == notificationManager)
                    {
                        return null;
                    }

                    notificationManager.CreateNotificationChannel(notificationChannel);
                }
            }

            return notificationChannel;
        }

        private PendingIntent? CreatePendingIntent(long requestCode, string action)
        {
            var intent = new Intent(action).SetPackage(service.PackageName);
            return PendingIntent.GetBroadcast(Application.Context, (int)requestCode, intent, PendingIntentFlags.CancelCurrent);
        }

        private static PendingIntent? CreateContentIntent()
        {
            var context = Application.Context;
            var className = Java.Lang.Class.FromType(typeof(MainActivity));
            var notificationIntent = new Intent(context, className);
            
            notificationIntent.SetAction(IIntents.Close);
            notificationIntent.SetFlags(ActivityFlags.ClearTop | ActivityFlags.SingleTop | ActivityFlags.NewTask);

            return PendingIntent.GetActivity(context, 0, notificationIntent, PendingIntentFlags.UpdateCurrent);
        }

        /// <summary>
        /// 
        /// </summary>
        public interface IIntents
        {
            public const string Play = IntentActionPlay;
            public const string Pause = IntentActionPause;
            public const string SkipToPrev = IntentActionSkipToPrev;
            public const string SkipToNext = IntentActionSkipToNext;
            public const string Close = IntentActionClose;
        }

        /// <summary>
        /// 
        /// </summary>
        public interface IChannels
        {
            public const string ChannelID = DefaultChannelId;
            public const int NotificationID = DefaultNotificationId;
        }
    }
}