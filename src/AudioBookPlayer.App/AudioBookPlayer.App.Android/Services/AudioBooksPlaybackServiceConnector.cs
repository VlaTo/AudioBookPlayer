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
using AudioBookPlayer.App.Android.Models;
using AudioBookPlayer.App.Models;
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
        private long activeQueueItemId;
        private int queueIndex;

        public bool IsConnected => mediaBrowser is { IsConnected: true };

        /*public int QueueIndex
        {
            get => queueIndex;
            private set
            {
                if (queueIndex != value)
                {
                    ;
                }

                queueIndex = value;
            }
        }*/

        public long PlaybackPosition
        {
            get;
            private set;
        }

        public long PlaybackDuration
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

                    queueIndex = index;

                    return;
                }

                queueIndex = -1;
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

        public PlaybackState PlaybackState
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

        /// <inheritdoc cref="IMediaBrowserServiceConnector.PlaybackStateChanged" />
        public event EventHandler PlaybackStateChanged
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

        /// <inheritdoc cref="IMediaBrowserServiceConnector.QueueIndexChanged" />
        public event EventHandler QueueIndexChanged
        {
            add => eventManager.AddEventHandler(value);
            remove => eventManager.RemoveEventHandler(value);
        }

        /// <inheritdoc cref="IMediaBrowserServiceConnector.CurrentMediaInfoChanged" />
        public event EventHandler CurrentMediaInfoChanged
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
                // OnExtrasChangedImpl = OnExtrasChanged,
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
            booksLibraryTask = new BooksLibraryTask(mediaBrowser, bookItemsCache);
            bookItemsTask = new BookItemsTask(booksLibraryTask, bookItemsCache);
            bookSectionsTask = new BookSectionsTask(mediaBrowser);

            Chapters = Array.Empty<IChapterMetadata>();
            AudioBookMetadata = null;
            ActiveQueueItemId = -1;
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
            var handler = new Handler(Looper.MainLooper, new ActionHandlerCallback(message =>
            {
                /*var temp1 = Handler.CreateAsync(Looper.MyLooper());
                var temp2 = temp1.ObtainMessage(1);

                temp1.DispatchMessage(temp2);*/

                System.Diagnostics.Debug.WriteLine("[ConnectionCallback.OnConnected] [Handler] Callback executed");
                tcs.Complete();
            }));

            MediaController.SendCommand(
                AudioBooksPlaybackService.IAudioBookMediaBrowserService.UpdateLibrary,
                Bundle.Empty,
                new ResultReceiver(handler)
            );

            return tcs.Task;
        }

        /*public void Play(EntityId bookId)
        {
            var controls = MediaController?.GetTransportControls();

            if (null != controls)
            {
                var mediaId = new MediaId(bookId).ToString();
                var options = new Bundle();
                controls.PlayFromMediaId(mediaId, options);
            }
        }*/

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

        private void OnPlaybackStateChanged(PlaybackStateCompat playback)
        {
            PlaybackState = playback.State.ToPlaybackState();
            ActiveQueueItemId = playback.ActiveQueueItemId;

            var start = GetMediaFragmentStart(playback.Extras);
            var duration = GetMediaFragmentDuration(playback.Extras);
            var queueId = GetMediaFragmentLong(playback.Extras, "Queue.ID");

            PlaybackPosition = playback.Position;
            PlaybackDuration = duration;
            
            eventManager.HandleEvent(this, EventArgs.Empty, nameof(PlaybackStateChanged));
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

            // eventManager.HandleEvent(this, EventArgs.Empty, nameof(ChaptersChanged));
        }

        private void OnSessionReady()
        {
            /*var messenger = new Messenger(new CallbackHandler
            {
                OnMessage = msg =>
                {
                    switch (msg.What)
                    {
                        case AudioBooksPlaybackService.ICustomPlayback.PositionChangedEvent:
                        {
                            var position = msg.Data.GetLong(AudioBooksPlaybackService.ICustomPlayback.PositionKey);
                            var duration = msg.Data.GetLong(AudioBooksPlaybackService.ICustomPlayback.DurationKey);

                            //System.Diagnostics.Debug.WriteLine($"[MediaControllerCallback.OnSessionReadyImpl] [Handle] Position: {position}");

                            PlaybackPosition = position;
                            PlaybackDuration = duration;

                            eventManager.HandleEvent(this, EventArgs.Empty, nameof(CurrentMediaInfoChanged));

                            break;
                        }

                        case AudioBooksPlaybackService.ICustomPlayback.QueueIndexChangedEvent:
                        {
                            var index = msg.Data.GetInt(AudioBooksPlaybackService.ICustomPlayback.QueueIndexKey);

                            QueueIndex = index;

                            eventManager.HandleEvent(this, EventArgs.Empty, nameof(QueueIndexChanged));

                            break;
                        }

                        default:
                        {
                            System.Diagnostics.Debug.WriteLine("[MediaControllerCallback.OnSessionReadyImpl] [Handle] Callback executed");
                            break;
                        }
                    }
                }
            });

            var options = new Bundle();

            options.PutParcelable("MESSENGER", messenger);

            MediaController.SendCommand(AudioBooksPlaybackService.IAudioBookMediaBrowserService.SubscribePlayback, options, null);*/
        }

        private void OnExtrasChanged(Bundle extras)
        {
            //QueueIndex = extras.GetInt("QueueIndex");
            //CurrentMediaPosition = extras.GetLong("CurrentMediaPosition");
            //eventManager.HandleEvent(this, EventArgs.Empty, nameof(QueueIndexChanged));
        }

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

        private sealed class CallbackHandler : Handler
        {
            public Action<Message> OnMessage
            {
                get; 
                set;
            }

            public CallbackHandler()
            {
                ;
            }

            public override void HandleMessage(Message msg) => OnMessage?.Invoke(msg);
        }

        /*public interface IPlaybackCallback
        {
            void 
        }*/
    }
}