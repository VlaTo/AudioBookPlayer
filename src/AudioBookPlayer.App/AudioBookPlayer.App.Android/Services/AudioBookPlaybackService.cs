using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V4.Media.Session;
using AndroidX.Media;

namespace AudioBookPlayer.App.Android.Services
{
    // https://developer.android.com/guide/topics/media/media-controls
    // https://github.com/android/uamp/blob/f60b902643407ba234a316abe91410da7c08a4af/common/src/main/java/com/example/android/uamp/media/MusicService.kt
    [Service]
    public sealed class AudioBookPlaybackService : MediaBrowserServiceCompat
    {
        private MediaSessionCompat mediaSession;
        private MediaControllerCompat mediaController;

        public MediaSessionCompat.Token SessionToken
        {
            get;
            private set;
        }

        public override BrowserRoot OnGetRoot(string clientPackageName, int clientUid, Bundle rootHints)
        {
            throw new System.NotImplementedException();
        }

        public override void OnLoadChildren(string parentId, Result result)
        {
            throw new System.NotImplementedException();
        }

        public override void OnCreate()
        {
            base.OnCreate();

            var componentName = new ComponentName(Application.Context, Class);
            var intent = PackageManager.GetLaunchIntentForPackage(componentName.PackageName);
            var pendingIntent = PendingIntent.GetActivity(Application.Context, 0, intent, PendingIntentFlags.NoCreate);

            mediaSession = new MediaSessionCompat(
                Application.Context,
                "com.libraprogramming.audiobook.player",
                componentName, pendingIntent)
            {
                Active = true
            };

            SessionToken = mediaSession.SessionToken;

            mediaController = new MediaControllerCompat(Application.Context, mediaSession);

            mediaController.RegisterCallback();
        }
    }
}