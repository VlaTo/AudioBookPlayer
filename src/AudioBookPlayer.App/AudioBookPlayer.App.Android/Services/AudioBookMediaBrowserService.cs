using Android.App;
using Android.Content;
using Android.Media.Session;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.Media;
using Android.Support.V4.Media.Session;
using AndroidX.Media;
using AudioBookPlayer.App.Android.Core;
using AudioBookPlayer.App.Android.Services.Builders;
using AudioBookPlayer.App.Domain.Services;
using AudioBookPlayer.App.Persistence.LiteDb;
using System;
using System.Threading;
using System.Threading.Tasks;
using Android.Provider;
using AudioBookPlayer.App.Android.Extensions;
using AudioBookPlayer.App.Core;
using AudioBookPlayer.App.Domain.Providers;
using AudioBookPlayer.App.Persistence.LiteDb.Extensions;
using AudioBookPlayer.App.Services;
using Application = Android.App.Application;

// https://developer.android.com/guide/topics/media/media-controls
// https://github.com/android/uamp/blob/f60b902643407ba234a316abe91410da7c08a4af/common/src/main/java/com/example/android/uamp/media/MusicService.kt
// https://github.com/xamarin/monodroid-samples/tree/master/android5.0/MediaBrowserService

namespace AudioBookPlayer.App.Android.Services
{
    /// <summary>
    /// Provides audiobooks library and handles playing them.
    /// </summary>
    [Service(Enabled = true, Exported = true)]
    [IntentFilter(new []{ ServiceInterface })]
    public sealed partial class AudioBookMediaBrowserService : MediaBrowserServiceCompat
    {
        private const string DatabaseFilename = "library.ldb";

        private readonly IBooksProvider booksProvider;
        private readonly ICoverService coverService;
        private readonly PackageValidator packageValidator;
        private readonly TaskExecutionMonitor<ResultReceiver> updateLibrary;
        private MediaSessionCompat mediaSession;
        private MediaControllerCompat mediaController;
        private PlaybackStateCompat.Builder playbackState;
        private MediaSessionCallback mediaSessionCallback;
        private MediaControllerCallback mediaControllerCallback;
        private BooksService booksService;

        // ReSharper disable once UnusedMember.Global
        public AudioBookMediaBrowserService()
            : this(
                AudioBookPlayerApplication.Instance.DependencyContainer.GetInstance<IBooksProvider>(),
                AudioBookPlayerApplication.Instance.DependencyContainer.GetInstance<ICoverService>()
            )
        {
        }

        private AudioBookMediaBrowserService(IBooksProvider booksProvider, ICoverService coverService)
        {
            this.booksProvider = booksProvider;
            this.coverService = coverService;

            packageValidator = new PackageValidator(Application.Context);
            updateLibrary = new TaskExecutionMonitor<ResultReceiver>(DoUpdateLibrary);
        }

        public override void OnCreate()
        {
            base.OnCreate();

            var componentName = new ComponentName(Application.Context, Class);
            var intent = PackageManager.GetLaunchIntentForPackage(componentName.PackageName);
            var pendingIntent = PendingIntent.GetActivity(Application.Context, 0, intent, PendingIntentFlags.UpdateCurrent);

            mediaSession = new MediaSessionCompat(Application.Context, nameof(AudioBookMediaBrowserService), componentName, pendingIntent);
            mediaSession.SetFlags((int)(MediaSessionFlags.HandlesMediaButtons | MediaSessionFlags.HandlesTransportControls));

            playbackState = new PlaybackStateCompat.Builder().SetActions(PlaybackStateCompat.ActionPlay | PlaybackStateCompat.ActionPlayPause);
            mediaSessionCallback = new MediaSessionCallback
            {
                OnCommandImpl = DoMediaSessionCommand,
                OnCustomActionImpl = DoMediaSessionCustomAction,
                OnPrepareImpl = DoMediaSessionPrepare
            };
            mediaSession.SetPlaybackState(playbackState.Build());
            mediaSession.SetCallback(mediaSessionCallback);
            mediaSession.SetMediaButtonReceiver(pendingIntent);

            SessionToken = mediaSession.SessionToken;

            mediaController = new MediaControllerCompat(Application.Context, mediaSession);
            mediaControllerCallback = new MediaControllerCallback();
            mediaController.RegisterCallback(mediaControllerCallback);

            var databasePath = GetDatabasePath(DatabaseFilename).AbsolutePath;
            var dbContext = new LiteDbContext(databasePath);

            booksService = new BooksService(dbContext, coverService);
        }

        public override BrowserRoot OnGetRoot(string clientPackageName, int clientUid, Bundle rootHints)
        {
            if (packageValidator.IsCallerAllowed(clientPackageName, clientUid))
            {
                if (null == rootHints || rootHints.IsEmpty)
                {
                    return new BrowserRoot(MediaPath.Root.ToString(), null);
                }

                if (rootHints.GetBoolean(BrowserRoot.ExtraRecent))
                {
                    var bundle = new Bundle();

                    bundle.PutBoolean(BrowserRoot.ExtraRecent, true);
                    bundle.PutBoolean("android.media.browse.SEARCH_SUPPORTED", true);

                    return new BrowserRoot(IAudioBookMediaBrowserService.Recent, bundle);
                }
            }

            return new BrowserRoot(IAudioBookMediaBrowserService.NoRoot, null);
        }

        // Root:             "/"                                (list of books)
        // Concrete book:    "/audiobook:1"                     (list of sections)
        // Concrete section: "/audiobook:1/section:0"           (list of chapters)
        // Concrete chapter: "/audiobook:1/section:0/chapter:0" ?
        public override void OnLoadChildren(string parentId, Result result)
        {
            if (String.Equals(parentId, MediaPath.Root.ToString()))
            {
                var audioBooks = booksService.QueryBooks();
                var list = new JavaList<MediaBrowserCompat.MediaItem>();

                for (var index = 0; index < audioBooks.Count; index++)
                {
                    var audioBook = audioBooks[index];
                    var mediaItem = audioBook.ToMediaItem();

                    list.Add(mediaItem);
                }

                result.SendResult(list);

                return;
            }

            //if (MediaBookId.TryParse(mediaPath[0], out var mediaBookId))
            if (MediaBookId.TryParse(parentId, out var bookId))
            {
                var audioBook = booksService.GetBook(bookId.EntityId);
                var list = new JavaList<MediaBrowserCompat.MediaItem>();

                for (var index = 0; index < audioBook.Sections.Count; index++)
                {
                    var section = audioBook.Sections[index];
                    var mediaItem = section.ToMediaItem();

                    list.Add(mediaItem);
                }

                result.SendResult(list);

                return;
            }

            /*if (parentId.StartsWith(IAudioBooksMediaBrowserService.LibraryRoot))
            {
                var path = parentId.Substring(IAudioBooksMediaBrowserService.LibraryRoot.Length);

                if (MediaBookId.TryParse(path, out var mediaBookId))
                {
                    var book = booksService.GetBook(mediaBookId.Id);
                    var builder = new PublicMediaItemBuilder();
                    var list = new JavaList<MediaBrowserCompat.MediaItem>();

                    for (var index = 0; index < book.Chapters.Count; index++)
                    {
                        var item = builder.BuildMediaItemFrom(
                            book.Chapters[index],
                            MediaBrowserCompat.MediaItem.FlagBrowsable | MediaBrowserCompat.MediaItem.FlagPlayable
                        );
                        list.Add(item);
                    }
                }

            }*/

            result.SendResult(null);
        }

        public override void OnDestroy()
        {
            base.OnDestroy();

            mediaSession.Active = false;
            mediaSession.Dispose();
        }

        private void DoMediaSessionCommand(string command, Bundle options, ResultReceiver result)
        {
            if (String.Equals(IAudioBookMediaBrowserService.UpdateLibrary, command))
            {
                updateLibrary.Start(result);
                return;
            }

            //var result = new Bundle();
            //result.PutBoolean("Result", true);
            result.Send(global::Android.App.Result.Canceled, null);
        }

        private async Task DoUpdateLibrary(ResultReceiver cb)
        {
            var result = Bundle.Empty;
            var booksLibrary = new AudioBooksLibrary();

            // 1. Get books from device and library
            var actualBooks = booksProvider.QueryBooks();
            var libraryBooks = booksService.QueryBooks();
            // 2. Compare collections, get differences
            var changes = booksLibrary.GetChanges(libraryBooks, actualBooks);
            // 3. Apply differences to library
            if (0 < changes.Count)
            {
                var success = await booksLibrary.TryApplyChangesAsync(booksService, changes, CancellationToken.None);

                if (success)
                {
                    result = new Bundle();

                    result.PutInt("Count", 1);

                    return;
                }
            }

            cb.Send(global::Android.App.Result.Ok, result);
        }

        private void DoMediaSessionCustomAction(string action, Bundle options)
        {
        }

        private void DoMediaSessionPrepare()
        {
            System.Diagnostics.Debug.WriteLine("[MediaSessionCallback] [OnPrepare] Execute");

            var metadata = new MediaMetadataCompat.Builder();
            metadata.PutString(MediaMetadataCompat.MetadataKeyMediaId, "media_1");
            metadata.PutString(MediaMetadataCompat.MetadataKeyAlbum, "Sample AudioBook");
            metadata.PutString(MediaMetadataCompat.MetadataKeyArtist, "Sample AudioBook Author");
            metadata.PutString(MediaMetadataCompat.MetadataKeyGenre, "Sample Genre");
            metadata.PutString(MediaMetadataCompat.MetadataKeyTitle, "Sample AudioBook Title");
            metadata.PutLong(MediaMetadataCompat.MetadataKeyDuration, (long)TimeSpan.FromHours(4.23d).TotalMilliseconds);

            mediaSession.SetMetadata(metadata.Build());

            if (false == mediaSession.Active)
            {
                mediaSession.Active = true;
            }
        }

        // MediaSessionCallback
        private sealed class MediaSessionCallback : MediaSessionCompat.Callback
        {
            public Action<string, Bundle, ResultReceiver> OnCommandImpl
            {
                get;
                set;
            }

            public Action<string, Bundle> OnCustomActionImpl
            {
                get;
                set;
            }

            public Action OnPrepareImpl
            {
                get;
                set;
            }

            public override void OnCommand(string command, Bundle options, ResultReceiver cb) => OnCommandImpl.Invoke(command, options, cb);

            public override void OnCustomAction(string action, Bundle extras) => OnCustomActionImpl.Invoke(action, extras);

            public override void OnPrepare() => OnPrepareImpl.Invoke();
        }

        // MediaControllerCallback class
        private sealed class MediaControllerCallback : MediaControllerCompat.Callback
        {
            public override void OnSessionReady()
            {
                System.Diagnostics.Debug.WriteLine("[AudioBookPlaybackService.MediaControllerCallback] [OnSessionReady] Execute");
            }

            public override void OnPlaybackStateChanged(PlaybackStateCompat state)
            {
                System.Diagnostics.Debug.WriteLine($"[AudioBookPlaybackService.MediaControllerCallback] [OnPlaybackStateChanged] Playback state: \"{state.State}\"");
            }

            public override void OnSessionEvent(string e, Bundle extras)
            {
                System.Diagnostics.Debug.WriteLine($"[AudioBookPlaybackService.MediaControllerCallback] [OnSessionEvent] Event: \"{e}\"");
            }


        }
    }
}