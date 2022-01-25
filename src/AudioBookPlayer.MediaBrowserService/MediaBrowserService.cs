#nullable enable

using Android.App;
using Android.Content;
using Android.Media.Session;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.Media;
using Android.Support.V4.Media.Session;
using AndroidX.Media;
using AudioBookPlayer.Core;
using AudioBookPlayer.Data.Persistence;
using AudioBookPlayer.Data.Persistence.Services;
using AudioBookPlayer.Domain;
using AudioBookPlayer.Domain.Models;
using AudioBookPlayer.Domain.Providers;
using AudioBookPlayer.Domain.Services;
using AudioBookPlayer.MediaBrowserService.Core;
using AudioBookPlayer.MediaBrowserService.Core.Internal;
using Java.Lang;
using System;
using System.Collections.Generic;
using System.Linq;
using Object = Java.Lang.Object;
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
        private BooksService? booksService;
        private ImageService? imageService;
        private MediaSessionCompat? mediaSession;

        public override void OnCreate()
        {
            base.OnCreate();

            dbContext = LiteDbContext.GetInstance(new PathProvider());
            imageService = new ImageService(ImageContentService.Instance);
            booksService = new BooksService(dbContext, imageService);

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

                return new BrowserRoot(MediaID.Root, Bundle.Empty);
            }

            return new BrowserRoot(MediaID.Empty, null);
        }

        public override void OnLoadChildren(string parentId, Result result)
        {
            if (String.Equals(MediaID.Root, parentId))
            {
                ProvideRoot(result);

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

                result.Detach();

                var manager = WorkLoadManager.Current();
                var booksProvider = new BooksProvider();
                var runnable = new UpdateLibraryRunnable(this, booksProvider, booksService, result);
                
                manager.Enqueue(runnable, extras);

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

        private void ProvideRoot(Result result)
        {
            if (null == booksService)
            {
                result.SendError(Bundle.Empty);
                return;
            }

            var list = new JavaList<MediaBrowserCompat.MediaItem>();
            var builder = new MediaItemBuilder();

            foreach (var book in booksService.QueryBooks())
            {
                var mediaItem = builder.CreateItem(book);
                list.Add(mediaItem);
            }

            result.SendResult(list);
        }

        private void ProvideChildren(MediaID mediaId, Result result)
        {
            if (null == booksService)
            {
                result.SendError(Bundle.Empty);
                return;
            }

            var list = new JavaList<MediaBrowserCompat.MediaItem>();
            /*var builder = new MediaItemBuilder();
            
            foreach (var book in booksService.QueryBooks())
            {
                var mediaItem = builder.CreateItem(book);
                list.Add(mediaItem);
            }*/

            result.SendResult(list);
        }

        public interface IMediaLibraryActions
        {
            public const string Update = LibraryUpdateAction;
        }

        public interface IUpdateLibrarySteps
        {
            public const int Scanning = UpdateStepCollecting;
            public const int Reading = UpdateStepCollecting;
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
            private readonly IBooksProvider booksProvider;
            private readonly IBooksService booksService;
            private readonly Result result;

            public UpdateLibraryRunnable(
                MediaBrowserService service,
                IBooksProvider booksProvider,
                IBooksService booksService,
                Result result)
            {
                this.service = service;
                this.booksProvider = booksProvider;
                this.booksService = booksService;
                this.result = result;

            }

            public void Run()
            {
                var reporter = new MultiStepProgress(new[]
                {
                    new ProgressStep(IUpdateLibrarySteps.Scanning, 0.6f),
                    new ProgressStep(IUpdateLibrarySteps.Reading, 0.1f),
                    new ProgressStep(IUpdateLibrarySteps.Processing, 0.3f)
                });

                var disposable = reporter.Subscribe((step, progress) =>
                {
                    var bundle = new Bundle();

                    bundle.PutInt("Update.Step", step);
                    bundle.PutFloat("Update.Progress", progress);

                    result.SendProgressUpdate(bundle);
                });

                try
                {
                    //var sourceBooks = booksProvider.QueryBooks(reporter[IUpdateLibrarySteps.Scanning]);
                    var sourceBooks = booksProvider.QueryBooks();
                    var targetBooks = booksService.QueryBooks();

                    var changes = GetLibraryChanges(sourceBooks, targetBooks);

                    ApplyLibraryChanges(changes, reporter[IUpdateLibrarySteps.Processing]);

                    service.NotifyChildrenChanged(MediaID.Root, Bundle.Empty);
                    result.SendResult(Bundle.Empty);
                }
                finally
                {
                    disposable.Dispose();
                }
            }

            private IReadOnlyList<ChangeInfo> GetLibraryChanges(IReadOnlyList<AudioBook> sourceBooks, IReadOnlyList<AudioBook> libraryBooks)
            {
                var changes = new List<ChangeInfo>();
                var delete = libraryBooks.ToList();

                for (var sourceIndex = 0; sourceIndex < sourceBooks.Count; sourceIndex++)
                {
                    var sourceBook = sourceBooks[sourceIndex];
                    var libraryIndex = FindIndex(libraryBooks, sourceBook);

                    if (-1 < libraryIndex)
                    {
                        var actualBook = libraryBooks[libraryIndex];

                        if (HasChanges(sourceBook, actualBook))
                        {
                            changes.Add(new ChangeInfo(ChangeAction.Update, sourceBook, actualBook));
                        }

                        delete.Remove(actualBook);
                    }
                    else
                    {
                        changes.Add(new ChangeInfo(ChangeAction.Add, sourceBook));
                    }
                }

                foreach (var audioBook in delete)
                {
                    changes.Add(new ChangeInfo(ChangeAction.Remove, audioBook));
                }

                return changes.AsReadOnly();
            }

            private void ApplyLibraryChanges(IReadOnlyList<ChangeInfo> changes, IProgress<float> progress)
            {
                for (var index = 0; index < changes.Count; index++)
                {
                    var change = changes[index];

                    switch (change.Action)
                    {
                        case ChangeAction.Add:
                        {
                            booksService.SaveBook(change.Source);

                            break;
                        }

                        case ChangeAction.Remove:
                        {
                            booksService.RemoveBook(change.Source);

                            break;
                        }

                        case ChangeAction.Update:
                        {
                            var bookId = change.Source.Id;

                            if (bookId.HasValue)
                            {
                                booksService.UpdateBook(bookId.Value, change.Target);
                            }

                            break;
                        }
                    }

                    progress.Report((float)(index + 1) / changes.Count);
                }
            }

            private static int FindIndex(IReadOnlyList<AudioBook> books, AudioBook book)
            {
                for (var index = 0; index < books.Count; index++)
                {
                    var actual = books[index];

                    if (actual.MediaId == book.MediaId)
                    {
                        return index;
                    }
                }

                return -1;
            }

            private static bool HasChanges(AudioBook sourceBook, AudioBook actualBook) =>
                sourceBook.Version != actualBook.Version;

            private enum ChangeAction
            {
                Add,
                Remove,
                Update
            }

            private readonly struct ChangeInfo
            {
                public ChangeAction Action
                {
                    get;
                }

                public AudioBook Source
                {
                    get;
                }

                public AudioBook? Target
                {
                    get;
                }

                public ChangeInfo(ChangeAction action, AudioBook source, AudioBook? target = null)
                {
                    Action = action;
                    Source = source;
                    Target = target;
                }
            }
        }
    }
}
