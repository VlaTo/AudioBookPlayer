#nullable enable

using Android.App;
using Android.Content;
using Android.Media.Session;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.Media;
using Android.Support.V4.Media.Session;
using AndroidX.Media;
using AudioBookPlayer.App.Android.Core;
using AudioBookPlayer.App.Android.Extensions;
using AudioBookPlayer.App.Android.Services.Helpers;
using AudioBookPlayer.App.Core;
using AudioBookPlayer.App.Domain.Extensions;
using AudioBookPlayer.App.Domain.Models;
using AudioBookPlayer.App.Domain.Providers;
using AudioBookPlayer.App.Domain.Services;
using AudioBookPlayer.App.Persistence.LiteDb;
using AudioBookPlayer.App.Services;
using System;
using System.Threading;
using System.Threading.Tasks;
using AudioBookPlayer.App.Domain.Core;
using Application = Android.App.Application;
using PlaybackState = Android.Media.Session.PlaybackState;

// https://developer.android.com/guide/topics/media/media-controls
// https://github.com/android/uamp/blob/f60b902643407ba234a316abe91410da7c08a4af/common/src/main/java/com/example/android/uamp/media/MusicService.kt
// https://github.com/xamarin/monodroid-samples/tree/master/android5.0/MediaBrowserService

namespace AudioBookPlayer.App.Android.Services
{
    /// <summary>
    /// Provides audiobooks library and handles playing them.
    /// </summary>
    [Service(Enabled = true, Exported = true)]
    [IntentFilter(new[] { ServiceInterface })]
    public sealed partial class AudioBooksPlaybackService : MediaBrowserServiceCompat, AudioBooksPlaybackService.IActions, AudioBooksPlaybackService.IKeys,
        AudioBooksPlaybackService.ICommands
    {
        private const string DatabaseFilename = "library.ldb";
        private const string ActionCommand = "Command";
        private const string ActionHint = "Hint";
        private const string KeyCommandName = "CommandName";
        private const string CommandPause = "Pause";

        private readonly IBooksProvider booksProvider;
        private readonly ICoverService coverService;
        private readonly PackageValidator packageValidator;
        private readonly TaskExecutionMonitor<ResultReceiver> updateLibrary;

        private MediaSessionCompat? mediaSession;
        private MediaSessionCallback? mediaSessionCallback;
        private BooksService? booksService;
        private NotificationService? notificationService;
        private PlaybackQueue? playbackQueue;
        private Playback? playback;

        // ReSharper disable once UnusedMember.Global
        public AudioBooksPlaybackService()
            : this(
                AudioBookPlayerApplication.Instance.DependencyContainer.GetInstance<IBooksProvider>(),
                AudioBookPlayerApplication.Instance.DependencyContainer.GetInstance<ICoverService>()
            )
        {
            ;
        }

        private AudioBooksPlaybackService(IBooksProvider booksProvider, ICoverService coverService)
        {
            this.booksProvider = booksProvider;
            this.coverService = coverService;

            packageValidator = new PackageValidator(Application.Context);
            updateLibrary = new TaskExecutionMonitor<ResultReceiver>(DoUpdateLibrary);
            mediaSession = null;
            mediaSessionCallback = null;
            booksService = null;
            playbackQueue = null;
            playback = null;
        }

        public override void OnCreate()
        {
            base.OnCreate();

            var componentName = new ComponentName(Application.Context, Class);
            var intent = PackageManager?.GetLaunchIntentForPackage(componentName.PackageName);
            var pendingIntent = PendingIntent.GetActivity(Application.Context, 0, intent, PendingIntentFlags.UpdateCurrent);

            playbackQueue = new PlaybackQueue();
            mediaSession = new MediaSessionCompat(Application.Context, nameof(AudioBooksPlaybackService), componentName, pendingIntent);
            mediaSession.SetFlags((int)(MediaSessionFlags.HandlesMediaButtons | MediaSessionFlags.HandlesTransportControls));

            mediaSessionCallback = new MediaSessionCallback
            {
                OnCommandImpl = DoMediaSessionCommand,
                OnCustomActionImpl = DoMediaSessionCustomAction,
                OnPrepareImpl = DoMediaSessionPrepare,
                OnPrepareFromMediaIdImpl = DoPrepareFromMediaId,
                OnPlayImpl = DoPlay,
                OnPlayFromMediaIdImpl = DoPlayFromMediaId,
                OnPauseImpl = DoPause
            };
            mediaSession.SetCallback(mediaSessionCallback);
            mediaSession.SetMediaButtonReceiver(pendingIntent);

            SessionToken = mediaSession.SessionToken;

            var databasePath = GetDatabasePath(DatabaseFilename)?.AbsolutePath;
            var dbContext = new LiteDbContext(databasePath);

            booksService = new BooksService(dbContext, coverService);
            playback = new Playback(this, mediaSession, booksService)
            {
                StateChanged = () =>
                {
                    System.Diagnostics.Debug.WriteLine($"[AudioBookMediaBrowserService.Playback] [StateChanged] State: {playback?.State}");
                    UpdatePlaybackState(-1, null);
                }
            };
            notificationService = new NotificationService(mediaSession.SessionToken);

            UpdatePlaybackState(-1, null);
        }

        public override StartCommandResult OnStartCommand(Intent? intent, StartCommandFlags flags, int startId)
        {
            if (null != intent)
            {
                var action = intent.Action;
                var command = intent.GetStringExtra(IKeys.CommandName);

                if (String.Equals(IActions.Command, action) && String.Equals(ICommands.Pause, command))
                {
                    if (playback is { IsPlaying: true })
                    {
                        HandlePauseRequest();
                    }
                }
            }

            return StartCommandResult.Sticky;
        }

        public override BrowserRoot OnGetRoot(string clientPackageName, int clientUid, Bundle rootHints)
        {
            if (packageValidator.IsCallerAllowed(clientPackageName, clientUid))
            {
                if (rootHints.IsEmpty)
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

        public override void OnLoadItem(string itemId, Result result)
        {
            System.Diagnostics.Debug.WriteLine($"[MediaBrowserService] [OnLoadItem] Item: \"{itemId}\"");
            base.OnLoadItem(itemId, result);
        }

        // Root:             "/"                                (list of books)
        // Concrete book:    "/audiobook:1"                     (list of sections)
        // Concrete section: "/audiobook:1/section:0"           (list of chapters)
        // Concrete chapter: "/audiobook:1/section:0/chapter:0" ?
        public override void OnLoadChildren(string parentId, Result result)
        {
            if (String.Equals(parentId, MediaPath.Root.ToString()))
            {
                var audioBooks = booksService?.QueryBooks();
                var list = new JavaList<MediaBrowserCompat.MediaItem>();

                if (null != audioBooks)
                {
                    for (var index = 0; index < audioBooks.Count; index++)
                    {
                        var audioBook = audioBooks[index];
                        var mediaItem = audioBook.ToMediaItem();

                        list.Add(mediaItem);
                    }
                }

                result.SendResult(list);

                return;
            }

            //if (MediaBookId.TryParse(mediaPath[0], out var mediaBookId))
            if (MediaBookId.TryParse(parentId, out var bookId))
            {
                var audioBook = booksService?.GetBook(bookId.EntityId);
                var list = new JavaList<MediaBrowserCompat.MediaItem>();

                if (null != audioBook)
                {
                    for (var index = 0; index < audioBook.Sections.Count; index++)
                    {
                        var section = audioBook.Sections[index];
                        var mediaItem = section.ToMediaItem();

                        list.Add(mediaItem);
                    }
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

            if (null != mediaSession)
            {
                mediaSession.SetMetadata(metadata.Build());

                if (false == mediaSession.Active)
                {
                    mediaSession.Active = true;
                }
            }
        }

        private void DoPrepareFromMediaId(string mediaId, Bundle options)
        {
            System.Diagnostics.Debug.WriteLine("[MediaSessionCallback] [DoPrepareFromMediaId] Execute");

            var metadata = new MediaMetadataCompat.Builder();

            metadata.PutString(MediaMetadataCompat.MetadataKeyMediaId, "media_1");
            metadata.PutString(MediaMetadataCompat.MetadataKeyAlbum, "Sample AudioBook");
            metadata.PutString(MediaMetadataCompat.MetadataKeyArtist, "Sample AudioBook Author");
            metadata.PutString(MediaMetadataCompat.MetadataKeyGenre, "Sample Genre");
            metadata.PutString(MediaMetadataCompat.MetadataKeyTitle, "Sample AudioBook Title");
            metadata.PutLong(MediaMetadataCompat.MetadataKeyDuration,
                (long)TimeSpan.FromHours(4.23d).TotalMilliseconds);

            if (null != mediaSession)
            {
                mediaSession.SetMetadata(metadata.Build());

                if (false == mediaSession.Active)
                {
                    mediaSession.Active = true;
                }
            }
        }

        private void DoPlay()
        {
            System.Diagnostics.Debug.WriteLine("[MediaSessionCallback] [DoPlay] Execute");

            if (playbackQueue is { IsEmpty: true })
            {
                playbackQueue.SetQueue(QueueHelper.GetLastPlaying());

                if (null != mediaSession)
                {
                    mediaSession.SetQueue(playbackQueue.AsReadOnlyList());
                    mediaSession.SetQueueTitle("Last Playing");
                }

                HandlePlayRequest();
            }
        }

        private void DoPlayFromMediaId(string mediaId, Bundle options)
        {
            System.Diagnostics.Debug.WriteLine($"[MediaSessionCallback] [DoPlayFromMediaId] MediaId: {mediaId}");

            if (playbackQueue.IsEmpty)
            {
                var bookId = MediaBookId.Parse(mediaId);

                playbackQueue.SetQueue(QueueHelper.GetQueue(booksService, bookId.EntityId));
                mediaSession.SetQueue(playbackQueue.AsReadOnlyList());
                mediaSession.SetQueueTitle("Last Playing");
            }
            else
            {
                ;
            }

            if (false == playbackQueue.IsEmpty)
            {
                HandlePlayRequest();
            }
        }

        private void DoPause()
        {
            HandlePauseRequest();
        }

        private void HandlePlayRequest()
        {
            if (false == playbackQueue.CanPlayCurrent)
            {
                return;
            }

            UpdateSessionMetadata();

            playback.Play(playbackQueue.Current);
        }

        private void HandlePauseRequest()
        {
            playback.Pause();
        }

        private long GetAvailableActions()
        {
            var actions = PlaybackState.ActionPlay | PlaybackState.ActionPlayFromMediaId;

            if (playbackQueue.IsEmpty)
            {
                return actions;
            }

            if (playback.IsPlaying)
            {
                actions |= PlaybackState.ActionPause;
            }

            if (0 < playbackQueue.CurrentIndex)
            {
                actions |= PlaybackState.ActionSkipToPrevious;
            }

            if ((playbackQueue.Count - 1) > playbackQueue.CurrentIndex)
            {
                actions |= PlaybackState.ActionSkipToNext;
            }

            return actions;
        }

        private void UpdateSessionMetadata()
        {
            if (false == playbackQueue.CanPlayCurrent)
            {
                return;
            }

            var queueItem = playbackQueue.Current;
            var mediaId = MediaBookId.Parse(queueItem.Description.MediaId);
            var audioBook = booksService.GetBook(mediaId.EntityId);
            var metadata = BuildMetadata(audioBook);

            mediaSession.SetMetadata(metadata);

            if (null == metadata.Description.IconBitmap && null != metadata.Description.IconUri)
            {
                var artUri = metadata.Description.IconUri.ToString();

                AlbumArtCache.Instance.Fetch(coverService, artUri, new FetchListener
                {
                    OnFetched = (uri, bitmap, icon) =>
                    {
                        var metadataBuilder = new MediaMetadataCompat.Builder(metadata);

                        metadataBuilder.PutBitmap(MediaMetadataCompat.MetadataKeyAlbumArt, bitmap);
                        metadataBuilder.PutBitmap(MediaMetadataCompat.MetadataKeyDisplayIcon, icon);

                        mediaSession.SetMetadata(metadataBuilder.Build());
                    }
                });
            }
        }

        private void UpdatePlaybackState(int errorCode, string? errorMessage)
        {
            var position = PlaybackStateCompat.PlaybackPositionUnknown;

            if (playback is { IsConnected: true })
            {
                position = playback.CurrentMediaPosition;
            }

            var playbackState = new PlaybackStateCompat.Builder().SetActions(GetAvailableActions());
            var state = playback.State;

            if (null != errorMessage)
            {
                playbackState.SetErrorMessage(errorCode, errorMessage);
                state = PlaybackStateCompat.StateError;
            }

            playbackState.SetState(state, position, 1.0f, SystemClock.ElapsedRealtime());

            if (playbackQueue.CanPlayCurrent)
            {
                playbackState.SetActiveQueueItemId(playbackQueue.Current.QueueId);
            }

            mediaSession.SetPlaybackState(playbackState.Build());

            if (PlaybackStateCompat.StatePlaying == playback.State || PlaybackStateCompat.StatePaused == playback.State)
            {
                notificationService?.ShowInformation(null);
            }
        }

        private static MediaMetadataCompat BuildMetadata(AudioBook audioBook)
        {
            var metadataBuilder = new MediaMetadataCompat.Builder();
            var mediaId = new MediaBookId(audioBook.Id);

            metadataBuilder.PutString(MediaMetadataCompat.MetadataKeyMediaId, mediaId.ToString());
            // metadataBuilder.PutString(MediaMetadataCompat.MetadataKeyAlbum, "Sample AudioBook");
            metadataBuilder.PutString(MediaMetadataCompat.MetadataKeyArtist, audioBook.Authors.AsString());
            // metadataBuilder.PutString(MediaMetadataCompat.MetadataKeyGenre, "Sample Genre");
            metadataBuilder.PutString(MediaMetadataCompat.MetadataKeyTitle, audioBook.Title);
            metadataBuilder.PutLong(MediaMetadataCompat.MetadataKeyDuration, (long)audioBook.Duration.TotalMilliseconds);

            for (var index = 0; index < audioBook.Images.Count; index++)
            {
                var audioBookImage = audioBook.Images[index];

                if (audioBookImage is IHasContentUri hcu)
                {
                    metadataBuilder.PutString(MediaMetadataCompat.MetadataKeyDisplayIconUri, hcu.ContentUri);
                    break;
                }
            }

            return metadataBuilder.Build();
        }

        public interface IActions
        {
            public const string Command = ActionCommand;
            public const string Hint = ActionHint;
        }
        
        public interface IKeys
        {
            public const string CommandName = KeyCommandName;
        }
        
        public interface ICommands
        {
            public const string Pause = CommandPause;
        }
    }
}