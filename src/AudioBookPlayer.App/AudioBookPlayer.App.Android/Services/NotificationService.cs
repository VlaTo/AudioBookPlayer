﻿#nullable enable

using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V4.Media.Session;
using AndroidX.Core.App;
using System;
using Application = Android.App.Application;

namespace AudioBookPlayer.App.Android.Services
{
    internal sealed class NotificationService : BroadcastReceiver, NotificationService.IIntents, NotificationService.IChannels
    {
        private const string DefaultChannelID = "AUDIOBOOKPLAYER_1";
        private const int DefaultNotificationID = 0x9000;
        
        public const string IntentActionPlay = "com.libraprogramming.audioplayer.actions.play";
        public const string IntentActionPause = "com.libraprogramming.audioplayer.actions.pause";
        public const string IntentActionSkipToPrev = "com.libraprogramming.audioplayer.actions.skiptoprev";
        public const string IntentActionSkipToNext = "com.libraprogramming.audioplayer.actions.skiptonext";
        public const string IntentActionClose = "com.libraprogramming.audioplayer.actions.close";

        private readonly AudioBooksPlaybackService service;
        private readonly MediaControllerCallback controllerCallback;
        private readonly NotificationManager? notificationManager;
        private readonly PendingIntent? closePendingIntent;
        private readonly PendingIntent? playIntent;
        private readonly PendingIntent? pauseIntent;
        private readonly PendingIntent? skipToPrevIntent;
        private readonly PendingIntent? skipToNextIntent;
        private MediaControllerCompat mediaController;
        private MediaControllerCompat.TransportControls controls;
        private NotificationChannel? notificationChannel;
        private MediaSessionCompat.Token? sessionToken;
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
                    mediaController.UnregisterCallback(controllerCallback);
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

        public NotificationService(AudioBooksPlaybackService service)
        {
            this.service = service;
            
            notificationManager = (NotificationManager?)Application.Context.GetSystemService(Context.NotificationService);

            var packageName = service.PackageName;

            playIntent = PendingIntent.GetBroadcast(service, 0, new Intent(IIntents.Play).SetPackage(packageName), PendingIntentFlags.CancelCurrent);
            pauseIntent = PendingIntent.GetBroadcast(service, 0, new Intent(IIntents.Pause).SetPackage(packageName), PendingIntentFlags.CancelCurrent);
            skipToPrevIntent = PendingIntent.GetBroadcast(service, 0, new Intent(IIntents.SkipToPrev).SetPackage(packageName), PendingIntentFlags.CancelCurrent);
            skipToNextIntent = PendingIntent.GetBroadcast(service, 0, new Intent(IIntents.SkipToNext).SetPackage(packageName), PendingIntentFlags.CancelCurrent);
            closePendingIntent = PendingIntent.GetBroadcast(service, 0, new Intent(IIntents.Close).SetPackage(packageName), PendingIntentFlags.CancelCurrent);

            controllerCallback = new MediaControllerCallback
            {
                OnPlaybackStateChangedImpl = playbackState =>
                {
                    if (null != playbackState && (PlaybackStateCompat.StateStopped == playbackState.State || PlaybackStateCompat.StateNone == playbackState.State))
                    {
                        HideInformation();
                    }
                    else
                    {
                        var notification = CreateNotification();

                        if (null != notification && null != notificationManager)
                        {
                            notificationManager.Notify(IChannels.NotificationID, notification);
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
                var notification = CreateNotification();

                if (null != notification)
                {
                    var filter = new IntentFilter();

                    filter.AddAction(IIntents.Play);
                    filter.AddAction(IIntents.Pause);
                    filter.AddAction(IIntents.SkipToPrev);
                    filter.AddAction(IIntents.SkipToNext);

                    mediaController.RegisterCallback(controllerCallback);
                    service.RegisterReceiver(this, filter);
                }

                shown = true;
            }
        }

        public void HideInformation()
        {
            if (shown)
            {
                shown = false;
                mediaController.UnregisterCallback(controllerCallback);

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

                // service.StopForeground(true);
            }
        }

        private bool TryAddSkipToNextAction(NotificationCompat.Builder? notificationBuilder)
        {
            var canSkipToNext = 0 != (mediaController.PlaybackState.Actions & PlaybackStateCompat.ActionSkipToNext);

            if (null != notificationBuilder && canSkipToNext)
            {
                var action = new NotificationCompat.Action(
                    Resource.Drawable.ic_skip_to_next,
                    Application.Context.Resources?.GetString(Resource.String.notification_action_next),
                    skipToNextIntent
                );

                notificationBuilder.AddAction(action);

                return true;
            }

            return false;
        }

        private bool TryAddSkipToPrevAction(NotificationCompat.Builder? notificationBuilder, ref int playActionIndex)
        {
            var canSkipToPrev = 0 != (mediaController.PlaybackState.Actions & PlaybackStateCompat.ActionSkipToPrevious);

            if (null != notificationBuilder && canSkipToPrev)
            {
                var action = new NotificationCompat.Action(
                    Resource.Drawable.ic_skip_to_prev,
                    Application.Context.Resources?.GetString(Resource.String.notification_action_prev),
                    skipToPrevIntent
                );

                notificationBuilder.AddAction(action);

                playActionIndex++;

                return true;
            }

            return false;
        }

        private bool TryAddPlayPauseAction(NotificationCompat.Builder? notificationBuilder)
        {
            const long mask = PlaybackStateCompat.ActionPlay | PlaybackStateCompat.ActionPlayPause | PlaybackStateCompat.ActionPlayFromMediaId;
            var canPlay = 0 != (mediaController.PlaybackState.Actions & mask);

            if (null != notificationBuilder && canPlay)
            {
                var playing = PlaybackStateCompat.StatePlaying == mediaController.PlaybackState.State;
                var action = new NotificationCompat.Action(
                    playing ? Resource.Drawable.ic_pause : Resource.Drawable.ic_play,
                    Application.Context.Resources?.GetString(
                        playing ? Resource.String.notification_action_pause : Resource.String.notification_action_play
                    ),
                    playing ? pauseIntent : playIntent
                );

                notificationBuilder.AddAction(action);

                return true;
            }

            return false;
        }

        private Notification? CreateNotification()
        {
            SessionToken = service.SessionToken;

            var nc = GetOrCreateNotificationChannel();
            var notificationBuilder = new NotificationCompat.Builder(Application.Context, nc.Id);

            var playActionIndex = 0;

            TryAddSkipToPrevAction(notificationBuilder, ref playActionIndex);
            TryAddSkipToNextAction(notificationBuilder);
            TryAddPlayPauseAction(notificationBuilder);

            // 3. create notification
            var mediaStyle = new AndroidX.Media.App.NotificationCompat.MediaStyle()
                .SetMediaSession(SessionToken)
                .SetShowActionsInCompactView(playActionIndex)
                .SetCancelButtonIntent(closePendingIntent)
                .SetShowCancelButton(true);

            var notification = notificationBuilder
                .SetStyle(mediaStyle)
                .SetSmallIcon(Resource.Drawable.ic_audiobookplayer)
                .SetContentTitle(mediaController.Metadata.Description.Title)
                .SetContentText("Sample Content Text")
                .SetContentIntent(mediaController.SessionActivity)
                .SetOnlyAlertOnce(true)
                .SetVisibility(NotificationCompat.VisibilityPublic)
                .Build();

            if (null != notification && null != notificationManager)
            {
                notificationManager.Notify(DefaultNotificationID, notification);
            }

            return notification;
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
            public const string ChannelID = DefaultChannelID;
            public const int NotificationID = DefaultNotificationID;
        }
    }
}