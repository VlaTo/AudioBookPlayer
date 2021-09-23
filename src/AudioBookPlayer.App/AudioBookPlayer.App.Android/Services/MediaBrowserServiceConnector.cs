using Android.Content;
using Android.OS;
using Android.Support.V4.Media;
using Android.Support.V4.Media.Session;
using AudioBookPlayer.App.Android.Services;
using AudioBookPlayer.App.Android.Services.Builders;
using AudioBookPlayer.App.Domain.Models;
using AudioBookPlayer.App.Services;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using LibraProgramming.Xamarin.Core;
using Xamarin.Forms;
using Application = Android.App.Application;
using ResultReceiver = Android.OS.ResultReceiver;

[assembly: Dependency(typeof(MediaBrowserServiceConnector))]

namespace AudioBookPlayer.App.Android.Services
{
    internal sealed class MediaBrowserServiceConnector : Java.Lang.Object, IMediaBrowserServiceConnector
    {
        private readonly MediaControllerCallback mediaControllerCallback;
        private readonly MediaBrowserCompat mediaBrowser;
        private MediaControllerCompat mediaController;
        private TaskCompletionSource connect;
        private TaskCompletionSource<IReadOnlyList<BookItem>> library;

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

        public MediaBrowserServiceConnector()
        {
            var serviceName = Java.Lang.Class.FromType(typeof(AudioBookMediaBrowserService)).Name;
            var componentName = new ComponentName(Application.Context, serviceName);
            var connectionCallback = new ServiceConnectionCallback(this, OnConnected);

            mediaBrowser = new MediaBrowserCompat(Application.Context, componentName, connectionCallback, null);
            mediaControllerCallback = new MediaControllerCallback(this);
        }

        /// <inheritdoc cref="IMediaBrowserServiceConnector.ConnectAsync" />
        public Task ConnectAsync()
        {
            var temp = new TaskCompletionSource();

            if (Interlocked.CompareExchange(ref connect, temp, null) == null)
            {
                System.Diagnostics.Debug.WriteLine("[MediaBrowserServiceConnector] [ConnectAsync] Starting connect");
                mediaBrowser.Connect();
            }

            return connect.Task;
        }

        /// <inheritdoc cref="IMediaBrowserServiceConnector.GetLibraryAsync" />
        public Task<IReadOnlyList<BookItem>> GetLibraryAsync()
        {
            var tcs = new TaskCompletionSource<IReadOnlyList<BookItem>>();

            if (null == Interlocked.CompareExchange(ref library, tcs, null))
            {
                var mediaId = mediaBrowser.Root;
                var options = new Bundle();
                var callback = new SubscriptionCallback(this);

                void Unsubscribe()
                {
                    mediaBrowser.Unsubscribe(mediaId, callback);
                    Interlocked.CompareExchange(ref library, null, tcs);
                }

                callback.ChildrenLoadedImpl = (parentId, children) =>
                {
                    var builder = new PreviewBookBuilder();
                    var bookItems = new BookItem[children.Count];

                    for (var index = 0; index < children.Count; index++)
                    {
                        var mediaItem = children[index];
                        bookItems[index] = builder.BuildBookFrom(mediaItem);
                    }

                    library.SetResult(bookItems);

                    Unsubscribe();
                };

                callback.ErrorImpl = parentId =>
                {
                    library.TrySetException(new Exception());
                    Unsubscribe();
                };

                mediaBrowser.Subscribe(mediaId, options, callback);
            }

            return library.Task;
        }

        public void Refresh()
        {
            // --- Sending command 'MEDIA_TEST_1' ---
            System.Diagnostics.Debug.WriteLine("[ConnectionCallback] [OnConnected] Sending command");

            var bundle = new Bundle();
            bundle.PutString("Test", "Value1");

            var handler = new Handler(Looper.MainLooper, new ActionHandlerCallback(message =>
            {
                System.Diagnostics.Debug.WriteLine("[ConnectionCallback.OnConnected] [Handler] Callback executed");
            }));
            var cb = new ResultReceiver(handler);

            mediaController.SendCommand("MEDIA_TEST_1", bundle, cb);
        }

        public void Temp(string mediaId)
        {
            var controls = MediaController.GetTransportControls();
            var options = new Bundle();

            controls.PrepareFromMediaId(mediaId, options);
        }

        /*public Task<CurrentBookViewModel> GetBookAsync(BookId id, ICoverService coverService)
        {
            var temp = new TaskCompletionSource<CurrentBookViewModel>();

            if (Interlocked.CompareExchange(ref getBook, temp, null) == null)
            {
                System.Diagnostics.Debug.WriteLine("[MediaBrowserServiceConnector] [GetBookAsync] Started");

                var mediaId = MediaPath.Combine(MediaPath.Root, id).ToAbsolute();
                var path = mediaId.ToString();
                var options = new Bundle();
                var callback = new MediaBrowserSubscriptionCallback(this);

                void Unsubscribe()
                {
                    mediaBrowser.Unsubscribe(path, callback);
                    Interlocked.CompareExchange(ref getBook, null, temp);

                    System.Diagnostics.Debug.WriteLine("[MediaBrowserServiceConnector] [GetBookAsync] Done");
                }

                callback.ChildrenLoadedImpl = (parentId, children) =>
                {
                    var builder = new PreviewBookBuilder();
                    var models = new BookItemViewModel[children.Count];

                    for (var index = 0; index < children.Count; index++)
                    {
                        var mediaItem = children[index];
                        models[index] = builder.BuildBookFrom(mediaItem);
                    }

                    Unsubscribe();

                    library.SetResult(models);
                };

                callback.ErrorImpl = parentId =>
                {
                    Unsubscribe();

                    library.TrySetException(new Exception());

                };

                mediaBrowser.Subscribe(path, options, callback);
            }

            return getBook.Task;
        }*/

        private void OnConnected(ConnectStatus status)
        {
            switch (status)
            {
                case ConnectStatus.Success:
                {
                    connect.Complete();
                    break;
                }

                case ConnectStatus.Cancelled:
                {
                    connect.Cancel();
                    break;
                }

                case ConnectStatus.Failed:
                {
                    connect.Fail(new Exception());
                    break;
                }
            }
        }

        // ConnectionCallback class
        private sealed class ServiceConnectionCallback : MediaBrowserCompat.ConnectionCallback
        {
            private readonly MediaBrowserServiceConnector connector;
            private readonly Action<ConnectStatus> connectCallback;

            public ServiceConnectionCallback(MediaBrowserServiceConnector connector, Action<ConnectStatus> connectCallback)
            {
                this.connector = connector;
                this.connectCallback = connectCallback;
            }

            public override void OnConnected()
            {
                var mediaController = new MediaControllerCompat(Application.Context, connector.SessionToken);

                connector.MediaController = mediaController;
                connectCallback.Invoke(ConnectStatus.Success);
            }

            public override void OnConnectionSuspended()
            {
                connectCallback.Invoke(ConnectStatus.Cancelled);
            }

            public override void OnConnectionFailed()
            {
                connectCallback.Invoke(ConnectStatus.Failed);
            }
        }

        private enum ConnectStatus
        {
            Failed = -1,
            Success,
            Cancelled
        }

        // ItemCallback class
        private sealed class ItemCallback : MediaBrowserCompat.ItemCallback
        {
            private readonly MediaBrowserServiceConnector connector;

            public ItemCallback(MediaBrowserServiceConnector connector)
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

        // SubscriptionCallback class
        /*private sealed class SubscriptionCallback : MediaBrowserCompat.SubscriptionCallback
        {
            private readonly MediaBrowserServiceConnector connector;
            private readonly IObserver<BookItemViewModel[]> observer;
            private readonly PreviewBookBuilder builder;

            public SubscriptionCallback(MediaBrowserServiceConnector connector, IObserver<BookItemViewModel[]> observer, ICoverService coverService)
            {
                this.connector = connector;
                this.observer = observer;

                builder = new PreviewBookBuilder(coverService);
            }

            public override void OnChildrenLoaded(string parentId, IList<MediaBrowserCompat.MediaItem> children)
            {
                var books = new BookItemViewModel[children.Count];

                for (var index = 0; index < children.Count; index++)
                {
                    var source = children[index];
                    books[index] = builder.BuildBookFrom(source);
                }

                observer.OnNext(books);
            }

            public override void OnError(string parentId)
            {
                observer.OnError(new Exception());
            }
        }*/

        private sealed class SubscriptionCallback : MediaBrowserCompat.SubscriptionCallback
        {
            private readonly MediaBrowserServiceConnector connector;

            public Bundle Options
            {
                get;
                private set;
            }

            public Action<string, IList<MediaBrowserCompat.MediaItem>> ChildrenLoadedImpl
            {
                get;
                set;
            }

            public Action<string> ErrorImpl
            {
                get;
                set;
            }

            public SubscriptionCallback(MediaBrowserServiceConnector connector)
            {
                this.connector = connector;

                ChildrenLoadedImpl = Stub.Empty<string, IList<MediaBrowserCompat.MediaItem>>();
                ErrorImpl = Stub.Empty<string>();
            }

            public override void OnChildrenLoaded(string parentId, IList<MediaBrowserCompat.MediaItem> children)
            {
                DoChildrenLoaded(parentId, children, Bundle.Empty);
            }

            public override void OnError(string parentId)
            {
                DoError(parentId, Bundle.Empty);
            }

            public override void OnChildrenLoaded(string parentId, IList<MediaBrowserCompat.MediaItem> children, Bundle options)
            {
                DoChildrenLoaded(parentId, children, options);
            }

            public override void OnError(string parentId, Bundle options)
            {
                DoError(parentId, options);
            }
            
            private void DoChildrenLoaded(string parentId, IList<MediaBrowserCompat.MediaItem> children, Bundle options)
            {
                Options = options;
                ChildrenLoadedImpl.Invoke(parentId, children);
            }

            private void DoError(string parentId, Bundle options)
            {
                Options = options;
                ErrorImpl.Invoke(parentId);
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

        // MediaControllerCallback class
        private sealed class MediaControllerCallback : MediaControllerCompat.Callback
        {
            private readonly MediaBrowserServiceConnector connector;

            public MediaControllerCallback(MediaBrowserServiceConnector connector)
            {
                this.connector = connector;
            }

            public override void OnPlaybackStateChanged(PlaybackStateCompat state)
            {
                System.Diagnostics.Debug.WriteLine("[MediaBrowserServiceConnector.MediaControllerCallback] [OnPlaybackStateChanged] Execute");
                base.OnPlaybackStateChanged(state);
            }

            public override void OnMetadataChanged(MediaMetadataCompat metadata)
            {
                System.Diagnostics.Debug.WriteLine("[MediaBrowserServiceConnector.MediaControllerCallback] [OnMetadataChanged] Execute");
            }

            public override void OnQueueChanged(IList<MediaSessionCompat.QueueItem> queue)
            {
                System.Diagnostics.Debug.WriteLine("[MediaBrowserServiceConnector.MediaControllerCallback] [OnQueueChanged] Execute");
            }

            public override void OnSessionDestroyed()
            {
                System.Diagnostics.Debug.WriteLine("[MediaBrowserServiceConnector.MediaControllerCallback] [OnSessionDestroyed] Execute");
            }

            public override void OnSessionEvent(string e, Bundle extras)
            {
                System.Diagnostics.Debug.WriteLine("[MediaBrowserServiceConnector.MediaControllerCallback] [OnSessionEvent] Execute");
            }

            public override void OnSessionReady()
            {
                System.Diagnostics.Debug.WriteLine("[MediaBrowserServiceConnector.MediaControllerCallback] [OnSessionReady] Execute");
            }
        }
    }
}