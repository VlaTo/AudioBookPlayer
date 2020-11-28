using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.Core.App;
using AudioBookPlayer.App.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AudioBookPlayer.App.Droid.Services
{
    internal sealed class AndroidPlaybackService : IPlaybackService
    {
        private readonly NotificationManagerCompat notificationManager;
        private readonly NotificationChannel channel;
        private readonly Notification notification;

        public AndroidPlaybackService()
        {
            notificationManager = NotificationManagerCompat.From(Application.Context);
            channel = new NotificationChannel("test", (string)null, NotificationImportance.Default);

            //notificationManager.CreateNotificationChannel(channel);

            var intent = new Intent(Application.Context, typeof(PlaybackActivity));
            var pendingIntent = PendingIntent.GetActivity(Application.Context, 0, intent, PendingIntentFlags.UpdateCurrent);

            notification = new NotificationCompat.Builder(Application.Context, channel.Id)
                .SetStyle(new NotificationCompat.BigTextStyle())
                .SetContentTitle("Sample Title")
                .SetContentText("Sample content text")
                .SetSmallIcon(Resource.Drawable.ic_mtrl_chip_checked_black)
                .SetContentIntent(pendingIntent)
                .Build();
        }

        public void ShowNotification()
        {
            //var intent = new Intent(Application.Context, typeof(MainActivity));
            //var pendingIntent = PendingIntent.GetActivity(Application.Context, 0, intent, PendingIntentFlags.UpdateCurrent);

            //var manager = NotificationManagerCompat.From(Application.Context);
            //var channel = new NotificationChannel("test", (string)null, NotificationImportance.Default);

            //manager.CreateNotificationChannel(channel);

            //var notification = new NotificationCompat.Builder(Application.Context, channel.Id)
            //    .SetStyle(new NotificationCompat.BigTextStyle())
            //    .SetContentTitle("Sample Title")
            //    .SetContentText("Sample content text")
            //    .SetSmallIcon(0)
            //    .SetContentIntent(pendingIntent)
            //    .Build();

            notificationManager.Notify(0, notification);
        }
    }
}