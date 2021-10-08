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
using AudioBookPlayer.App.Android.Models;
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
                OnPauseImpl = DoPause,
                OnSkipToQueueItemImpl = DoSkipToQueueItem
            };
            mediaSession.SetCallback(mediaSessionCallback);
            mediaSession.SetMediaButtonReceiver(pendingIntent);
            booksService = CreateBookService();

            SessionToken = mediaSession.SessionToken;

            var playbackCallback = new PlaybackCallback
            {
                StateChangedImpl = () =>
                {
                    if (playback is { State: PlaybackStateCompat.StateConnecting })
                    {
                        ;
                    }

                    UpdatePlaybackState(-1, null);
                }
            };

            playback = new Playback(this, mediaSession, playbackCallback);
            notificationService = new NotificationService(this);

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
            if (MediaId.TryParse(parentId, out var mediaId))
            {
                var audioBook = booksService?.GetBook(mediaId.BookId);
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

            if (null != mediaSession)
            {
                mediaSession.Active = false;
                mediaSession.Dispose();
                mediaSession = null;
            }
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
            throw new NotImplementedException();
        }

        private void DoMediaSessionPrepare()
        {
            throw new NotImplementedException();
        }

        private void DoPrepareFromMediaId(string mediaId, Bundle options)
        {
            var controllerInfo = mediaSession?.CurrentControllerInfo;
            var mid = MediaId.Parse(mediaId);

            if (null == playback || null == booksService)
            {
                return;
            }

            var audioBook = booksService.GetBook(mid.BookId);

            if (null != playbackQueue && null != audioBook)
            {
                var queue = QueueHelper.BuildQueue(audioBook);

                playbackQueue.SetQueue(queue);

                if (null != mediaSession)
                {
                    mediaSession.SetQueue(playbackQueue.AsQueue());
                    mediaSession.SetQueueTitle(audioBook.Title);
                    mediaSession.SetExtras(BuildQueueIndexExtras(playbackQueue.CurrentIndex));
                    
                    UpdateSessionMetadata(audioBook);
                }
            }
        }

        private void DoPlay()
        {
            if (playbackQueue is { IsEmpty: false })
            {
                var queueIndex = playbackQueue.CurrentIndex;

                if (playbackQueue.IsValidIndex(queueIndex) && null != playback)
                {
                    var mediaUri = playbackQueue.Current.Description.MediaUri;
                    var (start, duration) = BuildMediaFragment(playbackQueue.Current);

                    playback.PlayFragment(mediaUri, start, duration);
                }
            }
        }

        private void DoPlayFromMediaId(string mediaId, Bundle options)
        {
            throw new NotImplementedException();

            /*
            System.Diagnostics.Debug.WriteLine($"[MediaSessionCallback] [DoPlayFromMediaId] MediaId: {mediaId}");
             
            if (playbackQueue is { IsEmpty: true })
            {
                var id = MediaId.Parse(mediaId);

                //playbackQueue.SetQueue(QueueHelper.GetQueue(booksService, id.BookId));

                if (null != mediaSession)
                {
                    mediaSession.SetQueue(playbackQueue.AsReadOnlyList());
                    mediaSession.SetQueueTitle("Last Playing");
                }
            }
            else
            {
                ;
            }

            if (false == playbackQueue.IsEmpty)
            {
                HandlePlayRequest();
            }*/
        }

        private void DoPause()
        {
            HandlePauseRequest();
        }

        private void DoSkipToQueueItem(long queueId)
        {
            if (playbackQueue is { IsEmpty: false })
            {
                var queueItemIndex = playbackQueue.FindIndex(queueId);

                if (playbackQueue.IsValidIndex(queueItemIndex))
                {
                    playbackQueue.CurrentIndex = queueItemIndex;

                    if (null != playback)
                    {
                        var mediaUri = playbackQueue.Current.Description.MediaUri;
                        var (start, duration) = BuildMediaFragment(playbackQueue.Current);

                        playback.PlayFragment(mediaUri, start, duration);

                        /*var extras = new Bundle();
                        extras.PutInt("QueueIndex", playbackQueue.CurrentIndex);
    
                        mediaSession.SetExtras(extras);*/
                    }
                }

                /*if (null != mediaSession)
                {
                    mediaSession.SetQueue(playbackQueue.AsReadOnlyList());
                    mediaSession.SetQueueTitle("Last Playing");
                }*/
            }
        }

        /*private void HandlePlayRequest()
        {
            if (null != playbackQueue)
            {
                var queueIndex = playbackQueue.CurrentIndex;

                if (playbackQueue.IsValidIndex(queueIndex))
                {
                // UpdateSessionMetadata();

                if (null != playback)
                {
                    playback.PlayFragment(playbackQueue.Current);
                    playback.Play();
                }
            }
        }*/

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

        private void UpdateSessionMetadata(AudioBook audioBook)
        {
            if (null == playbackQueue || null == mediaSession)
            {
                return;
            }

            var metadata = BuildAudioBookMetadata(audioBook);

            mediaSession.SetMetadata(metadata);

            if (null == metadata.Description.IconBitmap && null != metadata.Description.IconUri)
            {
                var artUri = metadata.Description.IconUri.ToString();

                if (false == String.IsNullOrEmpty(artUri))
                {
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
        }

        private void UpdatePlaybackState(int errorCode, string? errorMessage)
        {
            var position = PlaybackStateCompat.PlaybackPositionUnknown;

            if (null == playback)
            {
                return;
            }

            if (playback.IsConnected)
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

            if (null != playbackQueue)
            {
                var queueIndex = playbackQueue.CurrentIndex;

                if (playbackQueue.IsValidIndex(queueIndex))
                {
                    playbackState.SetActiveQueueItemId(playbackQueue.Current.QueueId);
                }
            }

            mediaSession?.SetPlaybackState(playbackState.Build());

            if (PlaybackStateCompat.StatePlaying == playback.State || PlaybackStateCompat.StatePaused == playback.State)
            {
                notificationService?.ShowInformation();
            }
        }

        private BooksService CreateBookService()
        {
            var databasePath = GetDatabasePath(DatabaseFilename)?.AbsolutePath;
            var dbContext = new LiteDbContext(databasePath);

            return new BooksService(dbContext, coverService);
        }

        private static MediaMetadataCompat BuildAudioBookMetadata(AudioBook audioBook)
        {
            var metadataBuilder = new MediaMetadataCompat.Builder();
            var mediaId = new MediaId(audioBook.Id);

            metadataBuilder.PutString(MediaMetadataCompat.MetadataKeyMediaId, mediaId.ToString());
            metadataBuilder.PutString(MediaMetadataCompat.MetadataKeyArtist, audioBook.Authors.AsString());
            metadataBuilder.PutString(MediaMetadataCompat.MetadataKeyTitle, audioBook.Title);
            metadataBuilder.PutLong(MediaMetadataCompat.MetadataKeyDuration, (long)audioBook.Duration.TotalMilliseconds);
            // metadataBuilder.PutString(MediaMetadataCompat.MetadataKeyAlbum, "Sample AudioBook");
            // metadataBuilder.PutString(MediaMetadataCompat.MetadataKeyGenre, "Sample Genre");

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

        private static Bundle BuildQueueIndexExtras(int index)
        {
            var extras = new Bundle();
            
            extras.PutInt("QueueIndex", index);
            
            return extras;
        }

        private static MediaFragment BuildMediaFragment(MediaSessionCompat.QueueItem queueItem)
        {
            var start = queueItem.Description.Extras.GetDouble("Start");
            var duration = queueItem.Description.Extras.GetDouble("Duration");

            return new MediaFragment(start, duration);
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

        // PlaybackCallback
        private sealed class PlaybackCallback : IPlaybackCallback
        {
            public Action StateChangedImpl
            {
                get;
                set;
            }

            void IPlaybackCallback.StateChanged() => StateChangedImpl?.Invoke();
        }
    }
}