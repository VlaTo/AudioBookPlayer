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
using AudioBookPlayer.App.Android.Models;
using AudioBookPlayer.App.Android.Services.Helpers;
using AudioBookPlayer.App.Domain.Core;
using AudioBookPlayer.App.Domain.Extensions;
using AudioBookPlayer.App.Domain.Models;
using AudioBookPlayer.App.Domain.Services;
using AudioBookPlayer.App.Persistence.LiteDb;
using LibraProgramming.Xamarin.Core;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AudioBookPlayer.App.Core.Extensions;
using AudioBookPlayer.App.Domain.Providers;
using AudioBookPlayer.App.Services;
using Xamarin.Forms.Internals;
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
        private const string ActionCommand = "Command";
        private const string ActionHint = "Hint";
        private const string KeyCommandName = "CommandName";
        private const string CommandPause = "Pause";
        private const string ParamsPositionKey = "Media.CurrentPosition";
        private const string ParamsDurationKey = "Media.Duration";
        private const string ParamsQueueIndexKey = "Queue.CurrentIndex";

        private const int PlaybackPositionChangedEvent = 1;
        private const int PlaybackQueueIndexChangedEvent = 2;

        private ICoverService? coverService;
        private IBooksService? booksService;
        private IBooksProvider? booksProvider;
        private IActivityTrackerService? activityTrackerService;
        private readonly PackageValidator packageValidator;
        private readonly long seekDelta;

        private EntityId bookId;
        private bool resetFragment;
        private MediaSessionCompat? mediaSession;
        private MediaSessionCallback? mediaSessionCallback;
        private NotificationService? notificationService;
        private PlaybackQueue? playbackQueue;
        private Playback? playback;
        private PlaybackStateCompat.Builder? playbackStateBuilder;

        // ReSharper disable once UnusedMember.Global
        public AudioBooksPlaybackService()
        {
            packageValidator = new PackageValidator(Application.Context);
            bookId = EntityId.Empty;
            mediaSession = null;
            mediaSessionCallback = null;
            playbackQueue = null;
            playback = null;
            resetFragment = false;
            booksService = null;
            booksProvider = null;
            activityTrackerService = null;
            coverService = null;

            seekDelta = (long)TimeSpan.FromSeconds(20.0d).TotalMilliseconds;
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
            mediaSession.SetCallback(mediaSessionCallback);
            mediaSession.SetMediaButtonReceiver(pendingIntent);

            SessionToken = mediaSession.SessionToken;

            var playbackCallback = new PlaybackCallback
            {
                StateChangedImpl = DoPlaybackStateChanged,
                PositionChangedImpl = DoPlaybackPositionChanged,
                FragmentEndedImpl = DoFragmentEnded
            };

            CreateRequiredServices();

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
                var empty = booksService?.IsEmpty() ?? true;

                if (false == empty)
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
            }

            return new BrowserRoot(IAudioBookMediaBrowserService.NoRoot, null);
        }

        public override void OnLoadItem(string itemId, Result result)
        {
            System.Diagnostics.Debug.WriteLine($"[MediaBrowserService] [OnLoadItem] Item: \"{itemId}\"");
            base.OnLoadItem(itemId, result);
        }

        public override void OnLoadChildren(string parentId, Result result)
        {
            if (String.Equals(parentId, MediaPath.Empty.ToString()))
            {
                var list = new JavaList<MediaBrowserCompat.MediaItem>();

                result.SendResult(list);

                return;
            }

            if (String.Equals(parentId, MediaPath.Root.ToString()))
            {
                var audioBooks = booksService?.QueryBooks(new Progress<float>());
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

            if (String.Equals(parentId, IAudioBookMediaBrowserService.Recent))
            {
                var recent = activityTrackerService?.GetRecent();
                var list = new JavaList<MediaBrowserCompat.MediaItem>();

                if (null != recent && null != booksService)
                {
                    var audioBook = booksService.GetBook(recent.MediaId.BookId);

                    if (null != audioBook)
                    {
                        for (var index = 0; index < audioBook.Sections.Count; index++)
                        {
                            var section = audioBook.Sections[index];
                            var mediaItem = section.ToMediaItem();

                            list.Add(mediaItem);
                        }
                    }
                }

                result.SendResult(list);

                return;
            }

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

        public override void OnCustomAction(string action, Bundle extras, Result result)
        {
            if (String.Equals(IAudioBookMediaBrowserService.UpdateLibrary, action))
            {
                result.Detach();

                Task.Run(() => DoUpdateLibrary(extras, result)).RunAndForget();

                return;
            }

            base.OnCustomAction(action, extras, result);
        }

        private void DoUpdateLibrary(Bundle extras, Result result)
        {
            if (false == extras.IsEmpty)
            {
                ;
            }

            var actual = 0.0f;
            var library = 0.0f;
            var changes = 0.0f;
            var progressBundle = new Bundle();

            void UpdateProgress()
            {
                progressBundle.PutFloat("Progress", actual + library + changes);
                result.SendProgressUpdate(progressBundle);
            }

            // 1. Query books
            var actualBooks = booksProvider.QueryBooks(
                new Progress<float>(value =>
                {
                    actual = 0.4f * value;
                    UpdateProgress();
                })
            );
            var libraryBooks = booksService.QueryBooks(
                new Progress<float>(value =>
                {
                    library = 0.4f * value;
                    UpdateProgress();
                })
            );
            // 2. Compare collections, get differences
            var libraryChanges = GetChanges(libraryBooks, actualBooks);
            var data = new Bundle();
            data.PutInt("Changes", libraryChanges.Count);
            data.PutLong("Timestamp", DateTime.UtcNow.Ticks);
            // 3. Apply differences to library
            if (0 < libraryChanges.Count)
            {
                ApplyChanges(
                    libraryChanges,
                    new Progress<float>(value =>
                    {
                        changes = 0.2f * value;
                        UpdateProgress();
                    })
                );

                NotifyChildrenChanged(MediaPath.Root.ToString(), data);
            }

            result.SendResult(data);
        }

        private void DoPrepareFromMediaId(string mediaId, Bundle options)
        {
            var mid = MediaId.Parse(mediaId);

            if (null == playback || null == booksService)
            {
                return;
            }

            if (bookId == mid.BookId)
            {
                return;
            }

            bookId = mid.BookId;

            var audioBook = booksService.GetBook(bookId);

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

                var forceFragment = false;

                if (PlaybackStateCompat.StatePaused == playback.State)
                {
                    forceFragment = resetFragment;
                }
                else if (PlaybackStateCompat.StateStopped == playback.State ||
                         PlaybackStateCompat.StateNone == playback.State)
                {
                    forceFragment = true;
                }

                resetFragment = false;

                if (forceFragment)
                {
                    var queueIndex = playbackQueue.CurrentIndex;

                    if (playbackQueue.IsValidIndex(queueIndex))
                    {
                        HandlePlayRequest();
                    }
                }
                else
                {
                    playback.Play();
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
                }
            }
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
                        UpdatePlaybackPosition(playback.State, playback.MediaPosition);
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
                    if (playback.IsPlaying)
                    {
                        HandlePlayRequest();

                        return;
                    }

                    var (offset, _) = BuildMediaFragment(playbackQueue.Current);

                    if (null != playbackStateBuilder)
                    {
                        var extra = BuildMediaFragmentBundle(playbackQueue.Current);
                        playbackStateBuilder.SetExtras(extra);
                        UpdatePlaybackState(-1, null);
                        resetFragment = true;
                    }

                    UpdatePlaybackPosition(playback.State, offset);
                }
            }
        }

        private void DoSkipToNext()
        {
            if (playbackQueue is { IsEmpty: false } && playbackQueue.MoveNext())
            {
                if (null != playback)
                {
                    if (playback.IsPlaying)
                    {
                        HandlePlayRequest();

                        return;
                    }

                    var (offset, _) = BuildMediaFragment(playbackQueue.Current);

                    if (null != playbackStateBuilder)
                    {
                        var extra = BuildMediaFragmentBundle(playbackQueue.Current);
                        playbackStateBuilder.SetExtras(extra);
                        UpdatePlaybackState(-1, null);
                        resetFragment = true;
                    }

                    UpdatePlaybackPosition(playback.State, offset);
                }
            }
        }

        private void DoFastForward()
        {
            if (playbackQueue is { IsEmpty: false })
            {
                if (null != playback)
                {
                    var delta = seekDelta;
                    var left = playback.Duration - playback.Position;

                    if (delta > left)
                    {
                        delta -= left;

                        for (var queueIndex = playbackQueue.CurrentIndex + 1;
                            playbackQueue.IsValidIndex(queueIndex);
                            queueIndex++)
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
                    var delta = seekDelta;
                    var elapsed = playback.Position;

                    if (elapsed < delta)
                    {
                        delta -= elapsed;

                        for (var queueIndex = playbackQueue.CurrentIndex - 1;
                            playbackQueue.IsValidIndex(queueIndex);
                            queueIndex--)
                        {
                            var (_, duration) = BuildMediaFragment(playbackQueue[queueIndex]);

                            if (delta > duration)
                            {
                                delta -= duration;
                                continue;
                            }

                            playbackQueue.CurrentIndex = queueIndex;

                            HandlePlayRequest(duration - delta);

                            return;
                        }

                        playbackQueue.CurrentIndex = 0;

                        HandlePlayRequest();
                    }

                    if (null != mediaSession)
                    {
                        playback.SeekTo(-delta);
                    }
                }
            }
        }

        private void DoSeekTo(long position)
        {
            if (playbackQueue is { IsEmpty: false })
            {
                for (var queueIndex = 0; queueIndex < playbackQueue.Count; queueIndex++)
                {
                    var queueItem = playbackQueue[queueIndex];
                    var (offset, duration) = BuildMediaFragment(queueItem);
                    var delta = position - offset;

                    if (position >= offset && duration > delta)
                    {
                        playbackQueue.CurrentIndex = queueIndex;

                        HandlePlayRequest(delta);

                        return;
                    }
                }
            }
        }

        private void DoPlaybackPositionChanged()
        {
            if (null != playback)
            {
                UpdatePlaybackPosition(playback.State, playback.MediaPosition);
            }
        }

        private void DoPlaybackStateChanged()
        {
            UpdatePlaybackState(-1, null);

            if (null != playback && null != playbackQueue)
            {
                var queueItem = playbackQueue.Current;
                var mediaId = MediaId.Parse(queueItem.Description.MediaId);
                var position = new AudioBookPosition(mediaId, queueItem.QueueId, playback.MediaPosition);

                switch (playback.State)
                {
                    case PlaybackStateCompat.StatePlaying:
                    {
                        activityTrackerService.TrackActivity(ActivityType.Play, position);
                        break;
                    }

                    case PlaybackStateCompat.StatePaused:
                    {
                        activityTrackerService.TrackActivity(ActivityType.Pause, position);
                        break;
                    }

                    case PlaybackStateCompat.StateStopped:
                    {
                        activityTrackerService.TrackActivity(ActivityType.Stop, position);
                        break;
                    }
                }
            }
        }

        private void DoFragmentEnded()
        {
            if (playbackQueue is { IsEmpty: false } && playbackQueue.MoveNext())
            {
                if (playback is { IsPlaying: true })
                {
                    var position = playback.MediaPosition;
                    var (offset, duration) = BuildMediaFragment(playbackQueue.Current);

                    if (position > offset && position < (offset + duration))
                    {
                        return;
                    }
                }

                HandlePlayRequest();
            }
            else
            {
                DoStop();
            }
        }

        private static IReadOnlyList<LibraryChange> GetChanges(IReadOnlyList<AudioBook> libraryBooks, IReadOnlyList<AudioBook> actualBooks)
        {
            var changes = new List<LibraryChange>();
            var booksToAdd = new List<AudioBook>(actualBooks);

            for (var index = 0; index < libraryBooks.Count; index++)
            {
                var originalBook = libraryBooks[index];
                var actualIndex = FindBookIndex(actualBooks, originalBook);

                if (0 > actualIndex)
                {
                    changes.Add(LibraryChange.RemoveBook(originalBook));

                    continue;
                }

                var actualBook = actualBooks[actualIndex];

                if (IsChanged(originalBook, actualBook))
                {
                    changes.Add(LibraryChange.UpdateBook(actualBook));
                }

                booksToAdd.Remove(actualBook);
            }

            foreach (var book in booksToAdd)
            {
                changes.Add(LibraryChange.AddBook(book));
            }

            return changes;
        }

        private void ApplyChanges(IReadOnlyList<LibraryChange> changes, IProgress<float> progress)
        {
            for (var changeIndex = 0; changeIndex < changes.Count; changeIndex++)
            {
                var change = changes[changeIndex];

                switch (change.Action)
                {
                    case ChangeAction.Add:
                    {
                        booksService.SaveBook(change.Book);

                        break;
                    }

                    case ChangeAction.Remove:
                    {
                        booksService.RemoveBook(change.Book);

                        break;
                    }

                    default:
                    {
                        break;
                    }
                }
            }
        }

        private static int FindBookIndex(IReadOnlyList<AudioBook> books, AudioBook originalBook)
        {
            bool IsSameBook(AudioBook actualBook)
            {
                if (false == String.Equals(actualBook.Title, originalBook.Title))
                {
                    return false;
                }

                for (var index = 0; index < originalBook.Authors.Count; index++)
                {
                    if (actualBook.Authors.Contains(originalBook.Authors[index]))
                    {
                        continue;
                    }

                    return false;
                }

                return true;
            }

            return books.IndexOf(IsSameBook);
        }

        private static bool IsChanged(AudioBook originalBook, AudioBook actualBook)
        {
            if (originalBook.Authors.Count != actualBook.Authors.Count)
            {
                return true;
            }

            for (var authorIndex = 0; authorIndex < originalBook.Authors.Count; authorIndex++)
            {
                var originalBookAuthor = originalBook.Authors[authorIndex];

                if (String.Equals(originalBookAuthor, actualBook.Authors[authorIndex]))
                {
                    continue;
                }

                return true;
            }

            if (false == String.Equals(originalBook.Synopsis, actualBook.Synopsis))
            {
                return true;
            }

            return false;
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
                        UpdatePlaybackState(-1, null);
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
                resetFragment = false;
            }
        }

        private long GetAvailableActions()
        {
            var actions = PlaybackState.ActionPlay | PlaybackState.ActionSeekTo;

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

            if (bookId != EntityId.Empty)
            {
                if (null != playbackStateBuilder && null != playbackQueue)
                {
                    var extra = BuildMediaFragmentBundle(playbackQueue.Current);
                    playbackStateBuilder.SetExtras(extra);
                    UpdatePlaybackState(-1, null);
                }
            }

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
                position = playback.Offset + playback.Position;
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
                    playbackStateBuilder.SetActiveQueueItemId(playbackQueue.IsValidIndex(queueIndex) ? playbackQueue.Current.QueueId : -1L);
                }

                mediaSession?.SetPlaybackState(playbackStateBuilder.Build());
            }
        }

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
                    metadataBuilder.PutString(MediaMetadataCompat.MetadataKeyAlbumArtUri, hcu.ContentUri);
                    break;
                }
            }

            return metadataBuilder.Build();
        }

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

        private void CreateRequiredServices()
        {
            var dbPath = new DatabasePathProvider();
            var dbContext = new LiteDbContext(dbPath);
            var mediaInfoProviderFactory = new MediaInfoProviderFactory();

            coverService = new CoverService();
            booksService = new BooksService(dbContext, coverService);
            activityTrackerService = new ActivityTrackerService(dbContext);
            booksProvider = new BooksProvider(mediaInfoProviderFactory);
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