using Android.Content;
using Android.OS;
using Android.Support.V4.Media;
using Android.Support.V4.Media.Session;
using AudioBookPlayer.App.Android.Extensions;
using AudioBookPlayer.App.Android.Models;
using AudioBookPlayer.App.Core;
using AudioBookPlayer.App.Domain.Models;
using AudioBookPlayer.App.Models;
using AudioBookPlayer.App.Services;
using LibraProgramming.Xamarin.Core;
using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Threading.Tasks;
using AudioBookPlayer.App.Android.Core;
using Xamarin.Forms;
using Application = Android.App.Application;
using PlaybackState = AudioBookPlayer.App.Core.PlaybackState;

namespace AudioBookPlayer.App.Android.Services
{
    internal sealed partial class AudioBooksPlaybackServiceConnector : Java.Lang.Object, IMediaBrowserServiceConnector
    {
        private readonly IBookItemsCache bookItemsCache;
        private readonly MediaControllerCallback mediaControllerCallback;
        private readonly MediaBrowserCompat mediaBrowser;
        private readonly ServiceConnectTask serviceConnectTask;
        //private readonly BooksLibraryTask booksLibraryTask;
        //private readonly BookItemsTask bookItemsTask;
        //private readonly BookSectionsTask bookSectionsTask;
        private readonly WeakEventManager eventManager;
        private readonly List<(ILibraryCallback, IDisposable)> libraryCallbacks;
        private MediaControllerCompat mediaController;
        private long activeQueueItemId;
        private bool rootSubscribed;

        public bool IsConnected => mediaBrowser is { IsConnected: true };

        public long Offset
        {
            get;
            private set;
        }

        public long Position
        {
            get;
            private set;
        }

        public long Duration
        {
            get;
            private set;
        }

        public long ActiveQueueItemId
        {
            get => activeQueueItemId;
            private set
            {
                if (activeQueueItemId == value)
                {
                    return;
                }

                activeQueueItemId = value;

                var chapters = Chapters;

                for (var index = 0; index < chapters.Count; index++)
                {
                    if (chapters[index].QueueId != value)
                    {
                        continue;
                    }

                    return;
                }
            }
        }

        public MediaSessionCompat.Token SessionToken => mediaBrowser.SessionToken;

        public MediaControllerCompat MediaController
        {
            get => mediaController;
            private set
            {
                if (ReferenceEquals(mediaController, value))
                {
                    return;
                }

                if (null != mediaController)
                {
                    mediaController.UnregisterCallback(mediaControllerCallback);
                }

                mediaController = value;
                
                if (null != mediaController)
                {
                    mediaController.RegisterCallback(mediaControllerCallback);
                }
            }
        }

        public PlaybackState State
        {
            get;
            private set;
        }

        public IAudioBookMetadata AudioBookMetadata
        {
            get;
            private set;
        }

        public IReadOnlyList<IChapterMetadata> Chapters
        {
            get;
            private set;
        }

        /// <inheritdoc cref="IPlaybackController.StateChanged" />
        public event EventHandler StateChanged
        {
            add => eventManager.AddEventHandler(value);
            remove => eventManager.RemoveEventHandler(value);
        }

        /// <inheritdoc cref="IMediaBrowserServiceConnector.AudioBookMetadataChanged" />
        public event EventHandler AudioBookMetadataChanged
        {
            add => eventManager.AddEventHandler(value);
            remove => eventManager.RemoveEventHandler(value);
        }

        /// <inheritdoc cref="IMediaBrowserServiceConnector.ChaptersChanged" />
        public event EventHandler ChaptersChanged
        {
            add => eventManager.AddEventHandler(value);
            remove => eventManager.RemoveEventHandler(value);
        }

        public AudioBooksPlaybackServiceConnector(IBookItemsCache bookItemsCache)
        {
            this.bookItemsCache = bookItemsCache;

            var serviceName = Java.Lang.Class.FromType(typeof(AudioBooksPlaybackService)).Name;
            var componentName = new ComponentName(Application.Context, serviceName);
            TaskCompletionSource connect = null;
            var connectionCallback = new ServiceConnectionCallback
            {
                OnConnectImpl = () =>
                {
                    MediaController = new MediaControllerCompat(Application.Context, SessionToken);
                    connect?.Complete();
                },
                OnConnectionSuspendedImpl = () => connect?.Cancel(),
                OnConnectionFailedImpl = () => connect?.Fail(new Exception())
            };

            mediaBrowser = new MediaBrowserCompat(Application.Context, componentName, connectionCallback, null);
            mediaControllerCallback = new MediaControllerCallback
            {
                OnSessionReadyImpl = OnSessionReady,
                OnPlaybackStateChangedImpl = OnPlaybackStateChanged,
                OnMetadataChangedImpl = OnMetadataChanged,
                OnQueueChangedImpl = OnQueueChanged,
                OnQueueTitleChangedImpl = OnQueueTitleChanged,
                OnAudioInfoChangedImpl = pi =>
                {
                    System.Diagnostics.Debug.WriteLine($"[MediaBrowserServiceConnector.MediaControllerCallback] [OnAudioInfoChanged] Playback type: {pi.PlaybackType}");
                },
                OnSessionEventImpl = (eventName, options) =>
                {
                    System.Diagnostics.Debug.WriteLine($"[MediaBrowserServiceConnector.MediaControllerCallback] [OnSessionEvent] Event: '{eventName}' received");
                }
            };

            serviceConnectTask = new ServiceConnectTask
            {
                OnExecuteImpl = (tcs, cb) =>
                {
                    connect = tcs;
                    mediaBrowser.Connect();
                }
            };

            eventManager = new WeakEventManager();
            libraryCallbacks = new List<(ILibraryCallback, IDisposable)>();

            Chapters = Array.Empty<IChapterMetadata>();
            AudioBookMetadata = null;
            ActiveQueueItemId = -1L;
        }

        /// <inheritdoc cref="IMediaBrowserServiceConnector.ConnectAsync" />
        public Task ConnectAsync() => serviceConnectTask.ExecuteAsync();

        public IDisposable Subscribe(ILibraryCallback callback)
        {
            if (null == callback)
            {
                throw new ArgumentNullException(nameof(callback));
            }

            var subscription = Disposable.Create(() =>
            {
                // TODO: all incoming callbacks are same (from base viewmodel (BooksViewModelBase)) so, index found not always refers to correct callback
                var index = libraryCallbacks.FindIndex(tuple => tuple.Item1 == callback);
                libraryCallbacks.RemoveAt(index);
            });

            libraryCallbacks.Add((callback, subscription));

            EnsureSubscribeToMediaServiceRoot();

            return subscription;
        }

        /// <inheritdoc cref="IMediaBrowserServiceConnector.UpdateLibraryAsync" />
        public Task UpdateLibraryAsync()
        {
            var tcs = new TaskCompletionSource();
            var callback = new CustomActionCallback
            {
                OnProgressUpdateImpl = (action, extras, data) =>
                {
                    if (String.Equals(AudioBooksPlaybackService.IAudioBookMediaBrowserService.UpdateLibrary, action))
                    {
                        var progress = data.GetFloat("Progress", 0.0f);
                        System.Diagnostics.Debug.WriteLine($"[AudioBooksPlaybackServiceConnector.CustomActionCallback] [OnProgressUpdateImpl] Progress: {progress:P}");
                    }
                },
                OnResultImpl = (action, extras, data) =>
                {
                    var changes = data.GetInt("Changes", 0);
                    var ticks = data.GetLong("Timestamp", 0L);

                    if (0L < ticks)
                    {
                        var timestamp = new DateTime(ticks, DateTimeKind.Utc);
                        System.Diagnostics.Debug.WriteLine($"[AudioBooksPlaybackServiceConnector.CustomActionCallback] [OnResultImpl] Timestamp: {timestamp:s}");
                    }

                    tcs.Complete();
                }
            };

            mediaBrowser.SendCustomAction(
                AudioBooksPlaybackService.IAudioBookMediaBrowserService.UpdateLibrary,
                Bundle.Empty,
                callback
            );

            return tcs.Task;
        }

        public void Play()
        {
            var controls = MediaController?.GetTransportControls();

            if (null != controls)
            {
                controls.Play();
            }
        }

        public void Pause()
        {
            var controls = MediaController?.GetTransportControls();

            if (null != controls)
            {
                controls.Pause();
            }
        }

        public void PrepareFromMediaId(MediaId mediaId)
        {
            var controls = MediaController?.GetTransportControls();

            if (null != controls)
            {
                var options = Bundle.Empty;
                controls.PrepareFromMediaId(mediaId.ToString(), options);
            }
        }

        public void SetActiveQueueItemId(long queueItemId)
        {
            var controls = MediaController?.GetTransportControls();

            if (null != controls)
            {
                controls.SkipToQueueItem(queueItemId);
            }
        }

        public void SkipToPrevious()
        {
            var controls = MediaController?.GetTransportControls();

            if (null != controls)
            {
                controls.SkipToPrevious();
            }
        }

        public void SkipToNext()
        {
            var controls = MediaController?.GetTransportControls();

            if (null != controls)
            {
                controls.SkipToNext();
            }
        }

        public void FastForward()
        {
            var controls = MediaController?.GetTransportControls();

            if (null != controls)
            {
                controls.FastForward();
            }
        }

        public void Rewind()
        {
            var controls = MediaController?.GetTransportControls();

            if (null != controls)
            {
                controls.Rewind();
            }
        }

        public void SeekTo(long position)
        {
            var controls = MediaController?.GetTransportControls();

            if (null != controls)
            {
                controls.SeekTo(Offset + position);
            }
        }

        private void OnPlaybackStateChanged(PlaybackStateCompat psc)
        {
            State = psc.State.ToPlaybackState();
            ActiveQueueItemId = psc.ActiveQueueItemId;

            var offset = GetMediaFragmentStart(psc.Extras);
            var duration = GetMediaFragmentDuration(psc.Extras);
            var position = psc.Position - offset;
            //var queueId = GetMediaFragmentLong(psc.Extras, "Queue.ID");

            Offset = offset;
            Position = position;
            Duration = duration;
            
            eventManager.HandleEvent(this, EventArgs.Empty, nameof(StateChanged));
        }

        private static long GetMediaFragmentStart(Bundle bundle) => GetMediaFragmentLong(bundle, "Chapter.Start");

        private static long GetMediaFragmentDuration(Bundle bundle) => GetMediaFragmentLong(bundle, "Chapter.Duration");

        private static long GetMediaFragmentLong(Bundle bundle, string key)
        {
            if (null == bundle || false == bundle.ContainsKey(key))
            {
                return 0L;
            }

            return bundle.GetLong(key);
        }

        private void EnsureSubscribeToMediaServiceRoot()
        {
            if (rootSubscribed)
            {
                return;
            }

            rootSubscribed = true;

            //var mediaId = mediaBrowser.Root;
            var mediaId = MediaPath.Root.ToString();
            var subscriptionCallback = new SubscriptionCallback
            {
                OnChildrenLoadedImpl = (parentId, children, options) =>
                {
                    if (mediaId != parentId)
                    {
                        return;
                    }

                    var ticks = options.GetLong("Timestamp", 0L);

                    if (0L < ticks)
                    {
                        var timestamp = new DateTime(ticks, DateTimeKind.Utc);
                        System.Diagnostics.Debug.WriteLine($"[AudioBooksPlaybackServiceConnector.SubscriptionCallback] [OnChildrenLoaded] Timestamp: {timestamp:s}");
                    }

                    var bookItems = new BookItem[children.Count];

                    for (var index = 0; index < children.Count; index++)
                    {
                        var mediaItem = children[index];

                        bookItems[index] = mediaItem.ToBookItem();
                        // cache.Put(mediaItem.MediaId, bookItems[index]);
                    }

                    // notify callbacks
                    var handlers = libraryCallbacks.ToArray();

                    for (var index = 0; index < handlers.Length; index++)
                    {
                        var (cb, _) = handlers[index];

                        if (null != cb)
                        {
                            cb.OnGetBooks(bookItems);
                        }
                    }
                },
                OnErrorImpl = (parentId, options) =>
                {
                    ;
                }
            };

            var options = new Bundle();
            options.PutLong("Timestamp", DateTime.UtcNow.Ticks);

            mediaBrowser.Subscribe(mediaId, options, subscriptionCallback);
        }

        private void OnMetadataChanged(MediaMetadataCompat metadata)
        {
            // metadata.Description.MediaId;
            // metadata.Description.Title;
            // metadata.Description.Subtitle;
            // metadata.Description.Description;
            // metadata.Description.IconUri;
            // metadata.Description.Extras.GetLong("Duration");

            AudioBookMetadata = new AudioBookMetadata(metadata);

            eventManager.HandleEvent(this, EventArgs.Empty, nameof(AudioBookMetadataChanged));
        }

        private void OnQueueChanged(IList<MediaSessionCompat.QueueItem> queue)
        {
            var chapters = new List<IChapterMetadata>();

            for (var index = 0; index < queue.Count; index++)
            {
                var item = queue[index];
                chapters.Add(new ChapterMetadata(item));

                /*{
                    var id = item.QueueId;
                    var mediaId = item.Description.MediaId;
                    var title = item.Description.Title;
                    var start = TimeSpan.FromMilliseconds(item.Description.Extras.GetDouble("Start"));
                    var duration = TimeSpan.FromMilliseconds(item.Description.Extras.GetDouble("Duration"));
                    var sectionIndex = item.Description.Extras.GetInt("Index");
                    var sectionName = item.Description.Extras.GetString("Name");
                    var sectionContentUri = item.Description.Extras.GetString("ContentUri");
                }*/
            }

            Chapters = chapters.AsReadOnly();

            eventManager.HandleEvent(this, EventArgs.Empty, nameof(ChaptersChanged));
        }

        private void OnQueueTitleChanged(string queueTitle)
        {
        }

        private void OnSessionReady()
        {
        }

        private sealed class ActionHandler : Handler
        {
            private readonly Action<Message> handler;

            public ActionHandler(Action<Message> handler)
                : base(handler)
            {
                this.handler = handler;
            }

            public override void HandleMessage(Message msg)
            {
                handler.Invoke(msg);
            }
        }

        private sealed class CustomActionCallback : MediaBrowserCompat.CustomActionCallback
        {
            public Action<string, Bundle, Bundle> OnErrorImpl
            {
                get;
                set;
            }

            public Action<string, Bundle, Bundle> OnProgressUpdateImpl
            {
                get;
                set;
            }

            public Action<string, Bundle, Bundle> OnResultImpl
            {
                get;
                set;
            }

            public CustomActionCallback()
            {
                OnProgressUpdateImpl = Stub.Nop<string, Bundle, Bundle>();
                OnErrorImpl = Stub.Nop<string, Bundle, Bundle>();
                OnResultImpl = Stub.Nop<string, Bundle, Bundle>();
            }

            public override void OnError(
                string action,
                Bundle extras,
                Bundle data
            ) =>
                OnErrorImpl.Invoke(action, extras, data);

            public override void OnProgressUpdate(
                string action,
                Bundle extras,
                Bundle data
            ) =>
                OnProgressUpdateImpl.Invoke(action, extras, data);

            public override void OnResult(
                string action,
                Bundle extras,
                Bundle resultData
            ) =>
                OnResultImpl.Invoke(action, extras, resultData);
        }
    }
}