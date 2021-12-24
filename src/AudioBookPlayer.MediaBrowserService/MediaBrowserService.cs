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
using AudioBookPlayer.Core;
using AudioBookPlayer.Data.Persistence;
using AudioBookPlayer.Domain;
using AudioBookPlayer.MediaBrowserService.Core;
using AudioBookPlayer.MediaBrowserService.Core.Internal;
using Java.Lang;
using Java.Util.Concurrent;
using String = System.String;

namespace AudioBookPlayer.MediaBrowserService
{
    [Service(Enabled = true, Exported = true)]
    [IntentFilter(new []{ ServiceInterface })]
    public partial class MediaBrowserService : MediaBrowserServiceCompat, MediaBrowserService.IMediaLibraryActions, MediaBrowserService.IUpdateLibrarySteps
    {
        private const string SupportSearch = "android.media.browse.SEARCH_SUPPORTED";
        private const string LibraryUpdateAction = "com.libraprogramming.audioplayer.library.update";
        private const int UpdateStepCollecting = 1;
        private const int UpdateStepProcessing = 2;
        private const string ExtraRecent = "__RECENT__";

        private LiteDbContext? dbContext;
        private PackageValidator? packageValidator;
        private Callback? mediaSessionCallback;
        //private AudioBookDatabase? database;
        private MediaSessionCompat? mediaSession;
        //private MediaSessionCallback? mediaSessionCallback;

        public override void OnCreate()
        {
            base.OnCreate();

            dbContext = LiteDbContext.GetInstance(new PathProvider());

            var componentName = new ComponentName(Application.Context, Class);
            var intent = PackageManager?.GetLaunchIntentForPackage(componentName.PackageName);
            var pendingIntent = PendingIntent.GetActivity(Application.Context, 0, intent, PendingIntentFlags.UpdateCurrent);

            //playbackQueue = new PlaybackQueue();
            packageValidator = PackageValidator.Create(Application.Context);
            mediaSession = new MediaSessionCompat(Application.Context, nameof(MediaBrowserService), componentName, pendingIntent);
            mediaSession.SetFlags((int)(MediaSessionFlags.HandlesMediaButtons | MediaSessionFlags.HandlesTransportControls));

            mediaSessionCallback = new Callback
            {
                //OnCommandImpl = DoMediaSessionCommand,
                // OnCustomActionImpl = DoMediaSessionCustomAction,
                OnPrepareFromMediaIdImpl = DoPrepareFromMediaId,
                // OnPlayImpl = DoPlay,        // android 10+ playback resumption
                // OnPauseImpl = DoPause,
                // OnStopImpl = DoStop,
                // OnSkipToQueueItemImpl = DoSkipToQueueItem,
                // OnSkipToNextImpl = DoSkipToNext,
                // OnSkipToPreviousImpl = DoSkipToPrevious,
                // OnFastForwardImpl = DoFastForward,
                // OnRewindImpl = DoRewind,
                // OnSeekToImpl = DoSeekTo
            };
            mediaSession.SetCallback(mediaSessionCallback);
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

                /*var exists = 0 < GetBooksCount();

                if (exists)
                {
                    return new BrowserRoot(MediaID.Root, Bundle.Empty);
                }

                return new BrowserRoot(MediaID.Empty, Bundle.Empty);*/

                return new BrowserRoot(MediaID.Root, Bundle.Empty);
            }

            return new BrowserRoot(null, null);
        }

        public override void OnLoadChildren(string parentId, Result result)
        {
            if (String.Equals(MediaID.Empty, parentId))
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
                if (null == dbContext)
                {
                    var error = new Bundle(extras);

                    error.PutString("Error.Message", "");
                    result.SendError(error);

                    return;
                }

                var manager = WorkLoadManager.Current();

                result.Detach();
                manager.Enqueue(new UpdateLibraryRunnable(this, dbContext, result), extras);

                return;
            }

            base.OnCustomAction(action, extras, result);
        }

        private void DoPrepareFromMediaId(string mediaId, Bundle extra)
        {
            if (null == mediaSession)
            {
                return;
            }

            mediaSession.Active = true;

            var temp = new PlaybackStateCompat.Builder();
            temp.SetState((int)PlaybackStateCode.Stopped, 0L, 1.0f);

            mediaSession.SetPlaybackState(temp.Build());
            mediaSession.SetQueueTitle("Lorem Ipsum");
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
                if (null == dbContext)
                {
                    result.SendError(Bundle.Empty);
                    return;
                }

                using var scope = new UnitOfWork(dbContext);
                var books = scope.Books.All();

                foreach (var book in books)
                {
                    var itemId = new MediaID(book.Id);
                    var builder = new MediaDescriptionCompat.Builder();

                    builder.SetMediaId(itemId);
                    builder.SetTitle(book.Title);

                    var item = new MediaBrowserCompat.MediaItem(builder.Build(), (int)MediaItemFlags.Playable);

                    list.Add(item);
                }
            }

            result.SendResult(list);
        }

        private int GetBooksCount()
        {
            if (null == dbContext)
            {
                return 0;
            }

            using var scope = new UnitOfWork(dbContext);
            
            return scope.Books.Count();
        }

        public interface IMediaLibraryActions
        {
            public const string Update = LibraryUpdateAction;
        }

        public interface IUpdateLibrarySteps
        {
            public const int Collecting = UpdateStepCollecting;
            public const int Processing = UpdateStepProcessing;
        }

        private sealed class PathProvider : IPathProvider
        {
            public string GetPath(string filename)
            {
                var file = Application.Context.GetDatabasePath(filename);
                return file.AbsolutePath;
            }
        }

        /// <summary>
        /// UpdateLibraryRunnable class.
        /// </summary>
        private sealed class UpdateLibraryRunnable : Object, IRunnable
        {
            private readonly MediaBrowserService service;
            private readonly LiteDbContext dbContext;
            private readonly Result result;

            public UpdateLibraryRunnable(MediaBrowserService service, LiteDbContext dbContext, Result result)
            {
                this.service = service;
                this.dbContext = dbContext;
                this.result = result;

            }

            public void Run()
            {
                var reporter = new MultiStepProgress(new[]
                {
                    new ProgressStep(IUpdateLibrarySteps.Collecting, 0.7f),
                    new ProgressStep(IUpdateLibrarySteps.Processing, 0.3f)
                });

                var token = reporter.Subscribe((step, progress) =>
                {
                    var bundle = new Bundle();

                    bundle.PutInt("Update.Step", step);
                    bundle.PutFloat("Update.Progress", progress);

                    result.SendProgressUpdate(bundle);
                });

                try
                {
                    var millis = TimeUnit.Seconds.ToMillis(1L);
                    var booksProvider = new BooksProvider();
                    var sourceBooks = booksProvider.QueryBooks(reporter[IUpdateLibrarySteps.Collecting]);

                    const float total = 10.0f;
                    var processing = reporter[IUpdateLibrarySteps.Processing];

                    for (var position = 0; position < total; position++)
                    {
                        processing.Report((position + 1) / total);
                        Thread.Sleep(millis);
                    }

                    service.NotifyChildrenChanged(MediaID.Root, Bundle.Empty);
                    result.SendResult(Bundle.Empty);
                }
                finally
                {
                    token.Dispose();
                }
            }
        }
    }
}
