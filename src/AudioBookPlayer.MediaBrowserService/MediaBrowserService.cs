#nullable enable

using Android.App;
using Android.Content;
using Android.Media.Browse;
using Android.Media.Session;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.Media;
using Android.Support.V4.Media.Session;
using AndroidX.Media;
using AudioBookPlayer.Domain;
using AudioBookPlayer.MediaBrowserService.Core.Internal;
using System;

namespace AudioBookPlayer.MediaBrowserService
{
    [Service(Enabled = true, Exported = true)]
    [IntentFilter(new []{ ServiceInterface })]
    public class MediaBrowserService : MediaBrowserServiceCompat, MediaBrowserService.IMediaLibraryActions
    {
        private const string SupportSearch = "android.media.browse.SEARCH_SUPPORTED";
        private const string LibraryUpdateAction = "com.libraprogramming.audioplayer.library.update";
        private const string NoRoot = "@empty@";
        private const string ExtraRecent = "__RECENT__";

        private PackageValidator? packageValidator;
        //private AudioBookDatabase? database;
        private MediaSessionCompat? mediaSession;
        //private MediaSessionCallback? mediaSessionCallback;

        public override void OnCreate()
        {
            base.OnCreate();

            //database = AudioBookDatabase.GetDatabase(Application.Context);

            var componentName = new ComponentName(Application.Context, Class);
            var intent = PackageManager?.GetLaunchIntentForPackage(componentName.PackageName);
            var pendingIntent = PendingIntent.GetActivity(Application.Context, 0, intent, PendingIntentFlags.UpdateCurrent);

            //playbackQueue = new PlaybackQueue();
            packageValidator = PackageValidator.Create(Application.Context);
            mediaSession = new MediaSessionCompat(Application.Context, nameof(MediaBrowserService), componentName, pendingIntent);
            mediaSession.SetFlags((int)(MediaSessionFlags.HandlesMediaButtons | MediaSessionFlags.HandlesTransportControls));

            /*mediaSessionCallback = new MediaSessionCallback
            {
                //OnCommandImpl = DoMediaSessionCommand,
                // OnCustomActionImpl = DoMediaSessionCustomAction,
                OnPrepareFromMediaIdImpl = DoPrepareFromMediaId,
                OnPlayImpl = DoPlay,        // android 10+ playback resumption
                OnPauseImpl = DoPause,
                OnStopImpl = DoStop,
                OnSkipToQueueItemImpl = DoSkipToQueueItem,
                OnSkipToNextImpl = DoSkipToNext,
                OnSkipToPreviousImpl = DoSkipToPrevious,
                OnFastForwardImpl = DoFastForward,
                OnRewindImpl = DoRewind,
                OnSeekToImpl = DoSeekTo
            };
            mediaSession.SetCallback(mediaSessionCallback);*/
            mediaSession.SetMediaButtonReceiver(pendingIntent);

            // Установить активити, которая откроется, когда пользователь заинтерисуется подробностями этой сессии
            // var intent = new Intent(Application.Context, Java.Lang.Class.FromType(typeof(MainActivity)));
            // mediaSession.SetSessionActivity(PendingIntent.GetActivity(Application.Context, 0, intent, PendingIntentFlags.UpdateCurrent));

            SessionToken = mediaSession.SessionToken;
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
        }

        public override StartCommandResult OnStartCommand(Intent? intent, StartCommandFlags flags, int startId)
        {
            return base.OnStartCommand(intent, flags, startId);
        }

        public override BrowserRoot OnGetRoot(string clientPackageName, int clientUid, Bundle rootHints)
        {
            if (null != packageValidator && packageValidator.IsCallerAllowed(clientPackageName, clientUid))
            {
                if (rootHints.GetBoolean(BrowserRoot.ExtraRecent, false))
                {
                    var bundle = new Bundle();

                    bundle.PutBoolean(BrowserRoot.ExtraRecent, true);
                    bundle.PutBoolean(SupportSearch, false);

                    return new BrowserRoot(ExtraRecent, bundle);
                }

                return new BrowserRoot(MediaID.Root, Bundle.Empty);
            }

            return new BrowserRoot(null, null);
        }

        public override void OnLoadChildren(string parentId, Result result)
        {
            if (String.Equals(NoRoot, parentId))
            {
                result.SendResult(new JavaList<MediaBrowserCompat.MediaItem>());

                return;
            }

            if (String.Equals(ExtraRecent, parentId))
            {
                ProvideExtraRecent(result);

                return;
            }

            if (MediaID.TryParse(parentId, out var mediaId))
            {
                ProvideChildren(mediaId, result);

                return;
            }

            result.SendResult(null);
        }

        public override void OnCustomAction(string action, Bundle extras, Result result)
        {
            if (String.Equals(IMediaLibraryActions.Update, action))
            {
                ;
            }

            base.OnCustomAction(action, extras, result);
        }

        private void ProvideExtraRecent(Result result)
        {
            var list = new JavaList<MediaBrowserCompat.MediaItem>();
            result.SendResult(list);
        }

        private void ProvideChildren(MediaID mediaId, Result result)
        {
            var list = new JavaList<MediaBrowserCompat.MediaItem>();

            if (mediaId == MediaID.Root)
            {
                for (var index = 0; index < 10; index++)
                {
                    var itemId = new MediaID(101, index + 12);
                    var builder = new MediaDescriptionCompat.Builder();

                    builder.SetMediaId(itemId);
                    builder.SetTitle($"Audiobook #{index}");

                    var item = new MediaBrowserCompat.MediaItem(builder.Build(), (int)MediaItemFlags.Playable);

                    list.Add(item);
                }
            }

            result.SendResult(list);
        }

        public interface IMediaLibraryActions
        {
            public const string Update = LibraryUpdateAction;
        }
    }
}
