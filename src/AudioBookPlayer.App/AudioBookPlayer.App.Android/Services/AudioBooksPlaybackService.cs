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
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AudioBookPlayer.App.Android.Models;
using AudioBookPlayer.App.Domain.Core;
using LibraProgramming.Xamarin.Core;
using Application = Android.App.Application;
using PlaybackState = Android.Media.Session.PlaybackState;

// https://github.com/android/uamp/blob/main/docs/FullGuide.md
// https://developer.android.com/guide/topics/media/media-controls
// https://github.com/android/uamp/blob/f60b902643407ba234a316abe91410da7c08a4af/common/src/main/java/com/example/android/uamp/media/MusicService.kt
// https://github.com/xamarin/monodroid-samples/tree/master/android5.0/MediaBrowserService
// https://android-developers.googleblog.com/2020/08/playing-nicely-with-media-controls.html
// https://android.googlesource.com/platform/frameworks/base/+/master/core/java/android/os/Handler.java
// https://androidexperinz.wordpress.com/2012/02/14/communication-between-service-and-activity-part-1/
// https://androidexperinz.wordpress.com/2012/02/21/communication-between-service-and-activity-part-2/
// https://androidexperinz.wordpress.com/2012/12/14/communication-between-service-and-activity-part-3/

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
        private const string ParamsPositionKey = "Media.CurrentPosition";
        private const string ParamsDurationKey = "Media.Duration";
        private const string ParamsQueueIndexKey = "Queue.CurrentIndex";

        private const int PlaybackPositionChangedEvent = 1;
        private const int PlaybackQueueIndexChangedEvent = 2;

        private readonly IBooksProvider booksProvider;
        private readonly ICoverService coverService;
        private readonly PackageValidator packageValidator;
        private readonly TaskExecutionMonitor<ResultReceiver> updateLibrary;
        private readonly List<KeyValuePair<Bundle, ICustomPlayback>> callbacks;
        private readonly long playbackDelta;

        private MediaSessionCompat? mediaSession;
        private MediaSessionCallback? mediaSessionCallback;
        private BooksService? booksService;
        private NotificationService? notificationService;
        private PlaybackQueue? playbackQueue;
        private Playback? playback;
        private PlaybackStateCompat.Builder? playbackStateBuilder;

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
            callbacks = new List<KeyValuePair<Bundle, ICustomPlayback>>();
            mediaSession = null;
            mediaSessionCallback = null;
            booksService = null;
            playbackQueue = null;
            playback = null;

            playbackDelta = (long)TimeSpan.FromSeconds(20.0d).TotalMilliseconds;
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
                // OnCustomActionImpl = DoMediaSessionCustomAction,
                OnPrepareFromMediaIdImpl = DoPrepareFromMediaId,
                OnPlayImpl = DoPlay,        // android 10+ playback resumption
                OnPauseImpl = DoPause,
                OnStopImpl = DoStop,
                OnSkipToQueueItemImpl = DoSkipToQueueItem,
                OnSkipToNextImpl = DoSkipToNext,
                OnSkipToPreviousImpl = DoSkipToPrevious,
                OnFastForwardImpl = DoFastForward,
                OnRewindImpl = DoRewind
            };
            mediaSession.SetCallback(mediaSessionCallback);
            mediaSession.SetMediaButtonReceiver(pendingIntent);
            booksService = CreateBookService();

            SessionToken = mediaSession.SessionToken;

            var playbackCallback = new PlaybackCallback
            {
                StateChangedImpl = DoPlaybackStateChanged,
                PositionChangedImpl = DoPlaybackPositionChanged,
                FragmentEndedImpl = DoFragmentEnded
            };

            playbackStateBuilder = new PlaybackStateCompat.Builder();
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
                    bundle.PutBoolean("android.media.browse.SEARCH_SUPPORTED", false);

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
                result.Send(global::Android.App.Result.Ok, null);

                return;
            }
            
            if (String.Equals(IAudioBookMediaBrowserService.SubscribePlayback, command))
            {
                var messenger = (Messenger?)options.GetParcelable("MESSENGER");

                if (null != messenger)
                {
                    var callback = new CustomPlayback(messenger);
                    callbacks.Add(new KeyValuePair<Bundle, ICustomPlayback>(options, callback));
                }

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
                    
                    UpdateSessionMetadata(audioBook);
                    UpdatePlaybackState(-1, null);

                    // SendQueueIndex();
                    // SendPlaybackPosition();
                }
            }
        }

        private void DoPlay()
        {
            if (playbackQueue is { IsEmpty: false })
            {
                if (null == playback)
                {
                    return;
                }

                if (PlaybackStateCompat.StatePaused == playback.State)
                {
                    playback.Play();
                }
                else if (PlaybackStateCompat.StateStopped == playback.State ||
                         PlaybackStateCompat.StateNone == playback.State)
                {
                    var queueIndex = playbackQueue.CurrentIndex;

                    if (playbackQueue.IsValidIndex(queueIndex))
                    {
                        HandlePlayRequest();

                        /*var mediaUri = playbackQueue.Current.Description.MediaUri;
                        var (start, duration) = BuildMediaFragment(playbackQueue.Current);

                        playback.PlayFragment(mediaUri, start, duration);*/
                    }
                }
            }
        }

        private void DoPlayFromMediaId(string mediaId, Bundle options)
        {
            if (playbackQueue is { IsEmpty: false })
            {
                var queueIndex = playbackQueue.CurrentIndex;

                if (playbackQueue.IsValidIndex(queueIndex) && null != playback)
                {
                    HandlePlayRequest();

                    /*var mediaUri = playbackQueue.Current.Description.MediaUri;
                    var (start, duration) = BuildMediaFragment(playbackQueue.Current);

                    mediaSession.SetExtras(BuildQueueIndexExtras(playbackQueue.CurrentIndex));
                    playback.PlayFragment(mediaUri, start, duration);*/
                }
            }

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

        private void DoStop()
        {
            if (playback is { IsPlaying: true })
            {
                playback.Stop();
            }
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
                        var position = playback.MediaStart + playback.MediaPosition;
                        UpdatePlaybackPosition(playback.State, position);
                    }

                    HandlePlayRequest();
                }
            }
        }

        private void DoSkipToPrevious()
        {
            if (playbackQueue is { IsEmpty: false } && playbackQueue.MovePrevious())
            {
                if (null != playback)
                {
                    var position = playback.MediaStart + playback.MediaPosition;
                    UpdatePlaybackPosition(playback.State, position);
                }

                HandlePlayRequest();
            }
        }

        private void DoSkipToNext()
        {
            if (playbackQueue is { IsEmpty: false } && playbackQueue.MoveNext())
            {
                if (null != playback)
                {
                    var position = playback.MediaStart + playback.MediaPosition;
                    UpdatePlaybackPosition(playback.State, position);
                }

                HandlePlayRequest();
            }
        }

        private void DoFastForward()
        {
            if (playbackQueue is { IsEmpty: false })
            {
                if (null != playback)
                {
                    var delta = playbackDelta;
                    var left = playback.MediaDuration - playback.MediaPosition;

                    if (delta > left)
                    {
                        delta -= left;

                        for (var queueIndex = playbackQueue.CurrentIndex + 1; playbackQueue.IsValidIndex(queueIndex); queueIndex++)
                        {
                            var (_, duration) = BuildMediaFragment(playbackQueue[queueIndex]);

                            if (delta > duration)
                            {
                                delta -= duration;
                                continue;
                            }

                            playbackQueue.CurrentIndex = queueIndex;

                            HandlePlayRequest();

                            return;
                        }

                        DoStop();
                    }

                    if (null != mediaSession)
                    {
                        playback.SeekTo(delta);
                    }
                }

            }
        }

        private void DoRewind()
        {
            if (playbackQueue is { IsEmpty: false })
            {
                if (null != playback)
                {
                    var delta = playbackDelta;
                    var elapsed = playback.MediaPosition;

                    if (elapsed < delta)
                    {
                        delta -= elapsed;

                        for (var queueIndex = playbackQueue.CurrentIndex - 1; playbackQueue.IsValidIndex(queueIndex); queueIndex--)
                        {
                            var (_, duration) = BuildMediaFragment(playbackQueue[queueIndex]);

                            if (delta > duration)
                            {
                                delta -= duration;
                                continue;
                            }

                            playbackQueue.CurrentIndex = queueIndex;

                            HandlePlayRequest();

                            return;
                        }

                        MoveToPrev();
                    }

                    if (null != mediaSession)
                    {
                        playback.SeekTo(-delta);
                    }
                }
            }
        }

        private void DoPlaybackPositionChanged()
        {
            if (null != playback)
            {
                var position = playback.MediaStart + playback.MediaPosition;
                UpdatePlaybackPosition(playback.State, position);
            }
        }

        private void DoPlaybackStateChanged()
        {
            UpdatePlaybackState(-1, null);
        }

        private void DoFragmentEnded()
        {
            if (playbackQueue is { IsEmpty: false } && playbackQueue.MoveNext())
            {
                HandlePlayRequest();
            }
            else
            {
                DoStop();
            }
        }

        private void MoveToPrev()
        {
            if (null != playbackQueue)
            {
                playbackQueue.CurrentIndex = 0;

                var (_, duration) = BuildMediaFragment(playbackQueue.Current);

                HandlePlayRequest(duration - playbackDelta);
            }
        }

        private void HandlePlayRequest(long offset = 0L)
        {
            if (null != playbackQueue)
            {
                var mediaUri = playbackQueue.Current.Description.MediaUri;
                var (start, duration) = BuildMediaFragment(playbackQueue.Current);

                if (null != playback && null != mediaSession)
                {
                    playback.PlayFragment(mediaUri, start, duration, offset);

                    if (null != playbackStateBuilder)
                    {
                        var extra = BuildMediaFragmentBundle(playbackQueue.Current);
                        playbackStateBuilder.SetExtras(extra);
                    }
                }
            }
        }

        private void HandlePauseRequest()
        {
            if (null != playback)
            {
                playback.Pause();
            }
        }

        private long GetAvailableActions()
        {
            var actions = PlaybackState.ActionPlay | PlaybackState.ActionSeekTo /*| PlaybackState.ActionPlayFromMediaId*/;

            if (playbackQueue is { IsEmpty: true })
            {
                return actions;
            }

            if (playback is { IsPlaying: true })
            {
                actions |= PlaybackState.ActionPause;
            }

            if (null != playbackQueue)
            {
                if (0 < playbackQueue.CurrentIndex)
                {
                    actions |= PlaybackState.ActionSkipToPrevious;
                }

                if ((playbackQueue.Count - 1) > playbackQueue.CurrentIndex)
                {
                    actions |= PlaybackState.ActionSkipToNext;
                }
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
                position = playback.MediaStart + playback.MediaPosition;
            }

            if (null != playbackStateBuilder)
            {
                playbackStateBuilder.SetActions(GetAvailableActions());

                var state = playback.State;

                if (null != errorMessage)
                {
                    playbackStateBuilder.SetErrorMessage(errorCode, errorMessage);
                    state = PlaybackStateCompat.StateError;
                }

                /*if (null != playbackQueue)
                {
                    var queueIndex = playbackQueue.CurrentIndex;

                    if (playbackQueue.IsValidIndex(queueIndex))
                    {
                        var extra = BuildMediaFragmentBundle(playbackQueue.Current);
                        playbackStateBuilder.SetExtras(extra);
                    }
                }*/

                UpdatePlaybackPosition(state, position);

                if (PlaybackStateCompat.StatePlaying == playback.State ||
                    PlaybackStateCompat.StatePaused == playback.State)
                {
                    notificationService?.ShowInformation();
                }
            }
        }

        private void UpdatePlaybackPosition(int playbackState, long position)
        {
            if (null != playbackStateBuilder)
            {
                playbackStateBuilder.SetState(playbackState, position, Playback.DefaultSpeed, SystemClock.ElapsedRealtime());

                if (null != playbackQueue)
                {
                    var queueIndex = playbackQueue.CurrentIndex;

                    if (playbackQueue.IsValidIndex(queueIndex))
                    {
                        playbackStateBuilder.SetActiveQueueItemId(playbackQueue.Current.QueueId);
                    }
                }

                mediaSession?.SetPlaybackState(playbackStateBuilder.Build());
            }
        }

        private BooksService CreateBookService()
        {
            var databasePath = GetDatabasePath(DatabaseFilename)?.AbsolutePath;
            var dbContext = new LiteDbContext(databasePath);

            return new BooksService(dbContext, coverService);
        }

        /*private void SendPlaybackPosition()
        {
            if (null != playbackQueue)
            {
                var mediaPosition = 0L;
                var mediaDuration = 0L;

                if (playbackQueue.IsValidIndex(playbackQueue.CurrentIndex))
                {
                    if (null != playback)
                    {
                        mediaPosition = playback.MediaPosition;
                        mediaDuration = playback.MediaDuration;
                    }
                    else
                    {
                        var (_, duration) = BuildMediaFragment(playbackQueue.Current);
                        mediaDuration = (long)duration;
                    }
                }

                for (var index = 0; index < callbacks.Count; index++)
                {
                    var (_, cb) = callbacks[index];
                    cb.OnPlaybackPositionChanged(mediaPosition, mediaDuration);
                }
            }
        }*/

        /*private void SendQueueIndex()
        {
            if (null != playbackQueue)
            {
                for (var index = 0; index < callbacks.Count; index++)
                {
                    var (_, cb) = callbacks[index];
                    cb.OnQueueIndexChanged(playbackQueue.CurrentIndex);
                }
            }
        }*/

        private static MediaMetadataCompat BuildAudioBookMetadata(AudioBook audioBook)
        {
            var metadataBuilder = new MediaMetadataCompat.Builder();
            var mediaId = new MediaId(audioBook.Id);

            metadataBuilder.PutString(MediaMetadataCompat.MetadataKeyMediaId, mediaId.ToString());
            metadataBuilder.PutString(MediaMetadataCompat.MetadataKeyTitle, audioBook.Title);
            metadataBuilder.PutString(MediaMetadataCompat.MetadataKeyArtist, audioBook.Authors.AsString());
            metadataBuilder.PutLong(MediaMetadataCompat.MetadataKeyDuration, (long)audioBook.Duration.TotalMilliseconds);

            for (var index = 0; index < audioBook.Images.Count; index++)
            {
                var audioBookImage = audioBook.Images[index];

                if (audioBookImage is IHasContentUri hcu)
                {
                    //metadataBuilder.PutString(MediaMetadataCompat.MetadataKeyDisplayIconUri, hcu.ContentUri);
                    metadataBuilder.PutString(MediaMetadataCompat.MetadataKeyAlbumArtUri, hcu.ContentUri);
                    break;
                }
            }

            return metadataBuilder.Build();
        }

        /*private static Bundle BuildQueueIndexExtras(int index, long currentMediaPosition)
        {
            var extras = new Bundle();
            
            extras.PutInt("QueueIndex", index);
            extras.PutLong("CurrentMediaPosition", currentMediaPosition);
            
            return extras;
        }*/

        private static MediaFragment BuildMediaFragment(MediaSessionCompat.QueueItem queueItem)
        {
            var start = queueItem.Description.Extras.GetLong("Chapter.Start");
            var duration = queueItem.Description.Extras.GetLong("Chapter.Duration");

            return new MediaFragment(start, duration);
        }

        private static Bundle BuildMediaFragmentBundle(MediaSessionCompat.QueueItem queueItem)
        {
            var source = queueItem.Description.Extras;
            var start = source.GetLong("Chapter.Start");
            var duration = source.GetLong("Chapter.Duration");

            var bundle = new Bundle();

            bundle.PutLong("Chapter.Start", start);
            bundle.PutLong("Chapter.Duration", duration);
            bundle.PutLong("Queue.ID", queueItem.QueueId);

            return bundle;
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

            public Action PositionChangedImpl
            {
                get;
                set;
            }

            public Action FragmentEndedImpl
            {
                get;
                set;
            }

            public PlaybackCallback()
            {
                StateChangedImpl = Stub.Nop();
                PositionChangedImpl = Stub.Nop();
                FragmentEndedImpl = Stub.Nop();
            }

            void IPlaybackCallback.PositionChanged() => PositionChangedImpl.Invoke();
            
            public void FragmentEnded() => FragmentEndedImpl.Invoke();

            void IPlaybackCallback.StateChanged() => StateChangedImpl.Invoke();
        }

        // Playback
        private sealed class CustomPlayback : ICustomPlayback
        {
            private readonly Messenger messenger;

            public CustomPlayback(Messenger messenger)
            {
                this.messenger = messenger;
            }

            public void OnPlaybackPositionChanged(double position, double duration)
            {
                var bundle = new Bundle();

                bundle.PutDouble(ICustomPlayback.PositionKey, position);
                bundle.PutDouble(ICustomPlayback.DurationKey, duration);

                var message = new Message
                {
                    What = ICustomPlayback.PositionChangedEvent,
                    Data = bundle
                };

                messenger.Send(message);
            }

            public void OnQueueIndexChanged(int queueIndex)
            {
                var bundle = new Bundle();

                bundle.PutInt(ICustomPlayback.QueueIndexKey, queueIndex);

                var message = new Message
                {
                    What = ICustomPlayback.QueueIndexChangedEvent,
                    Data = bundle
                };

                messenger.Send(message);
            }
        }
    }
}