using Android.App;
using Android.Content;
using AndroidX.Core.App;
using AudioBookPlayer.App.Core.Services;

namespace AudioBookPlayer.App.Droid.Services
{
    internal sealed class AndroidPalaybackControlService : IPlaybackControlService
    {
        private readonly NotificationManagerCompat notificationManager;
        private readonly NotificationChannel channel;
        //private readonly Notification notification;
        private readonly PendingIntent pendingIntent;

        public AndroidPalaybackControlService()
        {
            notificationManager = NotificationManagerCompat.From(Application.Context);
            channel = new NotificationChannel("test", (string)null, NotificationImportance.Default);

            // will crash initialization
            //notificationManager.CreateNotificationChannel(channel);

            var intent = new Intent(Application.Context, typeof(PlaybackActivity));
            pendingIntent = PendingIntent.GetActivity(Application.Context, 0, intent, PendingIntentFlags.UpdateCurrent);

            /*notification = new NotificationCompat.Builder(Application.Context, channel.Id)
                .SetStyle(new NotificationCompat.BigTextStyle())
                .SetContentTitle("Sample Title")
                .SetContentText("Sample content text")
                .SetSmallIcon(Resource.Drawable.ic_mtrl_chip_checked_black)
                .SetContentIntent(pendingIntent)
                .Build();*/
        }
        
        public void ShowNotification()
        {
            if (notificationManager.AreNotificationsEnabled())
            {
                System.Diagnostics.Debug.WriteLine("[AndroidPlaybackService] [ShowNotification] Notifications disabled");

                return;
            }

            //var intent = new Intent(Application.Context, typeof(MainActivity));
            //var pendingIntent = PendingIntent.GetActivity(Application.Context, 0, intent, PendingIntentFlags.UpdateCurrent);

            //var manager = NotificationManagerCompat.From(Application.Context);
            //var channel = new NotificationChannel("test", (string)null, NotificationImportance.Default);

            //manager.CreateNotificationChannel(channel);

            var bigTextStyle = new NotificationCompat.BigTextStyle()
                .SetBigContentTitle("Big content title")
                .SetSummaryText("summary text");

            var notification = new NotificationCompat.Builder(Application.Context, channel.Id)
                .SetStyle(bigTextStyle)
                .SetContentTitle("content title")
                .SetContentText("content text")
                //.SetSmallIcon(Resource.Drawable.abc_ic_star_black_48dp)
                .SetSmallIcon(NotificationCompat.BadgeIconSmall)
                .SetContentIntent(pendingIntent)
                .SetDefaults(NotificationCompat.DefaultAll)
                .SetCategory(Notification.CategoryReminder)
                .Build();

            notificationManager.Notify(0, notification);
        }
    }
}