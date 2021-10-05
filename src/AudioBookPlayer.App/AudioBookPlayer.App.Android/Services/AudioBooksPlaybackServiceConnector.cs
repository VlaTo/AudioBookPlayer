using Android.Content;
using Android.OS;
using Android.Support.V4.Media;
using Android.Support.V4.Media.Session;
using AudioBookPlayer.App.Android.Extensions;
using AudioBookPlayer.App.Core;
using AudioBookPlayer.App.Domain.Models;
using AudioBookPlayer.App.Services;
using LibraProgramming.Xamarin.Core;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AudioBookPlayer.App.Android.Core;
using Xamarin.Forms;
using Application = Android.App.Application;
using PlaybackState = AudioBookPlayer.App.Core.PlaybackState;
using ResultReceiver = Android.OS.ResultReceiver;

namespace AudioBookPlayer.App.Android.Services
{
    internal sealed partial class AudioBooksPlaybackServiceConnector : Java.Lang.Object, IMediaBrowserServiceConnector
    {
        private readonly IBookItemsCache bookItemsCache;
        private readonly MediaControllerCallback mediaControllerCallback;
        private readonly MediaBrowserCompat mediaBrowser;
        private readonly ServiceConnectTask serviceConnectTask;
        private readonly BooksLibraryTask booksLibraryTask;
        private readonly BookItemsTask bookItemsTask;
        private readonly BookSectionsTask bookSectionsTask;
        private readonly WeakEventManager eventManager;
        private MediaControllerCompat mediaController;

        public bool IsConnected => mediaBrowser is { IsConnected: true };

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

        public PlaybackState PlaybackState
        {
            get;
            private set;
        }

        public IMediaMetadataDescription MediaMetadataDescription
        {
            get;
            private set;
        }

        /// <inheritdoc cref="IMediaBrowserServiceConnector.PlaybackStateChanged" />
        public event EventHandler PlaybackStateChanged
        {
            add => eventManager.AddEventHandler(value);
            remove => eventManager.RemoveEventHandler(value);
        }

        /// <inheritdoc cref="IMediaBrowserServiceConnector.MediaMetadataChanged" />
        public event EventHandler MediaMetadataChanged
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
                OnSessionReadyImpl = () =>
                {
                    System.Diagnostics.Debug.WriteLine("[MediaBrowserServiceConnector.MediaControllerCallback] [OnSessionReady] Executed");
                },
                OnPlaybackStateChangedImpl = OnPlaybackStateChanged,
                OnMetadataChangedImpl = OnMetadataChanged,
                OnQueueChangedImpl = OnQueueChanged,
                OnAudioInfoChangedImpl = (playbackInfo) =>
                {
                    System.Diagnostics.Debug.WriteLine("[MediaBrowserServiceConnector.MediaControllerCallback] [OnAudioInfoChanged] Execute");
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
            booksLibraryTask = new BooksLibraryTask(mediaBrowser, bookItemsCache);
            bookItemsTask = new BookItemsTask(booksLibraryTask, bookItemsCache);
            bookSectionsTask = new BookSectionsTask(mediaBrowser);
        }

        /// <inheritdoc cref="IMediaBrowserServiceConnector.ConnectAsync" />
        public Task ConnectAsync() => serviceConnectTask.ExecuteAsync();

        /// <inheritdoc cref="IMediaBrowserServiceConnector.GetLibraryAsync" />
        public Task<IReadOnlyList<BookItem>> GetLibraryAsync() => booksLibraryTask.ExecuteAsync();

        /// <inheritdoc cref="IMediaBrowserServiceConnector.GetBookItemAsync" />
        public Task<BookItem> GetBookItemAsync(EntityId bookId) => bookItemsTask.ExecuteAsync(bookId);

        /// <inheritdoc cref="IMediaBrowserServiceConnector.GetSectionsAsync"/>
        public Task<IReadOnlyList<SectionItem>> GetSectionsAsync(EntityId bookId) => bookSectionsTask.ExecuteAsync(bookId);

        /*{
            var tcs = new TaskCompletionSource<IReadOnlyList<SectionItem>>();

            if (AcquireSectionTask(bookId, tcs, out var current))
            {
                var mediaId = new MediaBookId(bookId).ToString();
                var callback = new SubscriptionCallback();

                void Unsubscribe()
                {
                    mediaBrowser.Unsubscribe(mediaId, callback);
                    ReleaseSectionTask(bookId, current);
                }

                callback.ChildrenLoadedImpl = (parentId, children) =>
                {
                    var sectionItems = new SectionItem[children.Count];

                    for (var index = 0; index < children.Count; index++)
                    {
                        var mediaItem = children[index];
                        sectionItems[index] = mediaItem.ToSectionItem();
                    }

                    current.SetResult(sectionItems);

                    Unsubscribe();
                };

                callback.ErrorImpl = parentId =>
                {
                    library.TrySetException(new Exception());
                    Unsubscribe();
                };

                var options = new Bundle();

                mediaBrowser.Subscribe(mediaId, options, callback);
            }

            return current.Task;
        }*/

        /// <inheritdoc cref="IMediaBrowserServiceConnector.UpdateLibraryAsync" />
        public Task UpdateLibraryAsync()
        {
            var tcs = new TaskCompletionSource();

            {
                var options = Bundle.Empty;
                var handler = new Handler(Looper.MainLooper, new ActionHandlerCallback(message =>
                {
                    System.Diagnostics.Debug.WriteLine("[ConnectionCallback.OnConnected] [Handler] Callback executed");
                    tcs.Complete();
                }));

                MediaController.SendCommand(
                    AudioBooksPlaybackService.IAudioBookMediaBrowserService.UpdateLibrary,
                    options,
                    new ResultReceiver(handler)
                );
            }

            return tcs.Task;
        }

        public void Play(EntityId bookId)
        {
            var controls = MediaController?.GetTransportControls();

            if (null != controls)
            {
                var mediaId = new MediaId(bookId).ToString();
                var options = new Bundle();
                controls.PlayFromMediaId(mediaId, options);
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

        private void OnPlaybackStateChanged(PlaybackStateCompat playback)
        {
            PlaybackState = playback.State.ToPlaybackState();
            eventManager.HandleEvent(this, EventArgs.Empty, nameof(PlaybackStateChanged));
        }

        private void OnMetadataChanged(MediaMetadataCompat metadata)
        {
            // metadata.Description.MediaId;
            // metadata.Description.Title;
            // metadata.Description.Subtitle;
            // metadata.Description.Description;
            // metadata.Description.IconUri;
            // metadata.Description.Extras.GetLong("Duration");

            MediaMetadataDescription = new AudioBookMediaMetadataDescription(metadata);
            eventManager.HandleEvent(this, EventArgs.Empty, nameof(MediaMetadataChanged));
        }

        private void OnQueueChanged(IList<MediaSessionCompat.QueueItem> queue)
        {
            for (var index = 0; index < queue.Count; index++)
            {
                var item = queue[index];

                {
                    var id = item.QueueId;
                    var mediaId = item.Description.MediaId;
                    var title = item.Description.Title;
                    var start = TimeSpan.FromMilliseconds(item.Description.Extras.GetDouble("Start"));
                    var duration = TimeSpan.FromMilliseconds(item.Description.Extras.GetDouble("Duration"));
                    var sectionIndex = item.Description.Extras.GetInt("Index");
                    var sectionName = item.Description.Extras.GetString("Name");
                    var sectionContentUri = item.Description.Extras.GetString("ContentUri");
                }
            }
        }

        // ConnectionCallback class
        private sealed class ServiceConnectionCallback : MediaBrowserCompat.ConnectionCallback
        {
            public Action OnConnectImpl
            {
                get;
                set;
            }

            public Action OnConnectionSuspendedImpl
            {
                get;
                set;
            }

            public Action OnConnectionFailedImpl
            {
                get;
                set;
            }

            public override void OnConnected() => OnConnectImpl.Invoke();

            public override void OnConnectionSuspended() => OnConnectionSuspendedImpl.Invoke();

            public override void OnConnectionFailed() => OnConnectionFailedImpl.Invoke();
        }

        // ItemCallback class
        private sealed class ItemCallback : MediaBrowserCompat.ItemCallback
        {
            private readonly AudioBooksPlaybackServiceConnector connector;

            public ItemCallback(AudioBooksPlaybackServiceConnector connector)
            {
                this.connector = connector;
            }

            public override void OnError(string itemId)
            {
                System.Diagnostics.Debug.WriteLine($"[ItemCallback] [OnError] Item: \"{itemId}\"");
            }

            public override void OnItemLoaded(MediaBrowserCompat.MediaItem item)
            {
                System.Diagnostics.Debug.WriteLine($"[ItemCallback] [OnItemLoaded] Loaded: \"{item.MediaId}\"");
            }
        }

        // ActionHandlerCallback class
        private sealed class ActionHandlerCallback : Java.Lang.Object, Handler.ICallback
        {
            private readonly Action<Message> handler;

            public ActionHandlerCallback(Action<Message> handler)
            {
                this.handler = handler;
            }

            public bool HandleMessage(Message msg)
            {
                handler.Invoke(msg);
                return true;
            }
        }
    }
}