using Android.App;
using Android.OS;
using Android.Support.V4.Media;
using Android.Support.V4.Media.Session;
using AndroidX.Core.App;
using AndroidX.Media;
using AudioBookPlayer.App.Android.Services.Builders;

// https://developer.android.com/guide/topics/media/media-controls
// https://github.com/android/uamp/blob/f60b902643407ba234a316abe91410da7c08a4af/common/src/main/java/com/example/android/uamp/media/MusicService.kt

namespace AudioBookPlayer.App.Android.Services
{
    [Service(Enabled = true, Exported = true)]
    [IntentFilter(new []{ "android.media.browse.MediaBrowserService" })]
    // ReSharper disable once UnusedMember.Global
    public sealed class AudioBookPlaybackService : MediaBrowserServiceCompat
    {
        private MediaSessionCompat mediaSession;
        private MediaControllerCompat mediaController;
        private NotificationBuilder notificationBuilder;
        private NotificationManagerCompat notificationManager;

        public MediaSessionCompat.Token SessionToken
        {
            get;
            private set;
        }

        public override BrowserRoot OnGetRoot(string clientPackageName, int clientUid, Bundle rootHints)
        {
            System.Diagnostics.Debug.WriteLine($"[{nameof(AudioBookPlaybackService)}.OnGetRoot] Requestor package: \"{clientPackageName}\"; client: {clientUid}");

            if (null != rootHints)
            {
                if (rootHints.GetBoolean(BrowserRoot.ExtraRecent))
                {
                    var bundle = new Bundle();
                    bundle.PutBoolean(BrowserRoot.ExtraRecent, true);
                    bundle.PutBoolean("android.media.browse.SEARCH_SUPPORTED", true);
                    return new BrowserRoot("__RECENT__", bundle);
                }

                return new BrowserRoot("/", null);
            }

            return new BrowserRoot("@empty@", null);
        }

        public override void OnLoadChildren(string parentId, Result result)
        {
            System.Diagnostics.Debug.WriteLine($"[{nameof(AudioBookPlaybackService)}.OnLoadChildren] parent id: \"{parentId}\"");

            var desc = new MediaDescriptionCompat.Builder()
                .SetTitle("Sample title")
                .SetSubtitle("Sample subtitle")
                .SetDescription("Sample description")
                .Build();

            var item = new MediaBrowserCompat.MediaItem(desc, MediaBrowserCompat.MediaItem.FlagBrowsable);

            result.SendResult(item);
        }

        public override void OnCreate()
        {
            System.Diagnostics.Debug.WriteLine("[AudioBookPlaybackService] [OnCreate] Executing");

            base.OnCreate();

            /*var componentName = new ComponentName(Application.Context, Class);
            var intent = PackageManager.GetLaunchIntentForPackage(componentName.PackageName);
            var pendingIntent = PendingIntent.GetActivity(Application.Context, 0, intent, PendingIntentFlags.NoCreate);

            mediaSession = new MediaSessionCompat(
                Application.Context,
                componentName.PackageName, //"com.libraprogramming.audiobook.player",
                componentName, pendingIntent)
            {
                Active = true
            };

            SessionToken = mediaSession.SessionToken;

            mediaController = new MediaControllerCompat(Application.Context, mediaSession);
            mediaController.RegisterCallback(new MediaControllerCallback());

            notificationBuilder = new NotificationBuilder();
            notificationManager = NotificationManagerCompat.From(Application.Context);*/

        }

        public override void OnDestroy()
        {
            base.OnDestroy();

            mediaSession.Active = false;
            mediaSession.Dispose();
        }

        // MediaControllerCallback class
        private sealed class MediaControllerCallback : MediaControllerCompat.Callback
        {
            public override void OnPlaybackStateChanged(PlaybackStateCompat state)
            {
                System.Diagnostics.Debug.WriteLine($"[{nameof(MediaControllerCallback)}.OnPlaybackStateChanged] state: \"{state.State}\"");

                base.OnPlaybackStateChanged(state);
            }
        }
    }
}