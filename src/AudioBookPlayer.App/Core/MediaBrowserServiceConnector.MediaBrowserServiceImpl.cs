using Android.Content;
using Android.OS;
using Android.Support.V4.Media;
using Android.Support.V4.Media.Session;
using System;
using System.Collections.Generic;

namespace AudioBookPlayer.App.Core
{
    internal sealed partial class MediaBrowserServiceConnector
    {
        /// <summary>
        /// 
        /// </summary>
        internal interface IAudioBooksCallback
        {
            void OnAudioBooksReady(IList<MediaBrowserCompat.MediaItem> list, Bundle options);

            void OnAudioBooksError(Bundle options);
        }

        internal interface IHistoryCallback
        {
            void OnHistoryReady(IList<MediaBrowserCompat.MediaItem> list, Bundle options);

            void OnHistoryError(Bundle options);
        }

        internal interface IUpdateCallback
        {
            void OnUpdateProgress(int step, float progress);

            void OnUpdateResult();
        }

        /// <summary>
        /// 
        /// </summary>
        internal interface IMediaServiceObserver
        {
            void OnMetadataChanged(MediaMetadataCompat metadata);

            void OnQueueTitleChanged(string title);

            void OnQueueChanged(IList<MediaSessionCompat.QueueItem> queue);
        }

        /// <summary>
        /// 
        /// </summary>
        internal interface IMediaBrowserService
        {
            void GetAudioBooks(IAudioBooksCallback callback);

            void UpdateLibrary(IUpdateCallback callback);

            void PrepareFromMediaId(string mediaId, Bundle extra);

            void GetHistory(IHistoryCallback callback);

            IDisposable Subscribe(IMediaServiceObserver observer);
        }

        /// <summary>
        /// 
        /// </summary>
        private sealed partial class MediaBrowserServiceImpl : IMediaBrowserService
        {
            private readonly Context context;
            private readonly MediaBrowserCompat mediaBrowser;
            private readonly MediaBrowserSubscriptionCallback subscriptionCallbacks;
            private readonly List<IAudioBooksCallback> booksCallbacks;
            private readonly List<Subscription> subscriptions;
            private readonly MediaControllerCompat mediaController;
            private IList<MediaBrowserCompat.MediaItem> books;
            private bool subscribed;

            public MediaBrowserServiceImpl(Context context, MediaBrowserCompat mediaBrowser)
            {
                this.context = context;
                this.mediaBrowser = mediaBrowser;

                subscribed = false;
                books = null;
                booksCallbacks = new List<IAudioBooksCallback>();
                subscriptions = new List<Subscription>();
                subscriptionCallbacks = new MediaBrowserSubscriptionCallback
                {
                    OnChildrenLoadedImpl = DoOnChildrenLoaded,
                    OnErrorImpl = DoOnError
                };

                var callbacks = new MediaControllerCallback
                {
                    OnMetadataChangedImpl = DoMetadataChanged,
                    // OnPlaybackStateChangedImpl = DoPlaybackStateChanged,
                    OnQueueChangedImpl = DoQueueChanged,
                    OnQueueTitleChangedImpl = DoQueueTitleChanged
                };

                mediaController = new MediaControllerCompat(context, mediaBrowser.SessionToken);
                mediaController.RegisterCallback(callbacks);
            }

            public void GetAudioBooks(IAudioBooksCallback callback)
            {
                if (false == booksCallbacks.Contains(callback))
                {
                    booksCallbacks.Add(callback);
                }

                if (subscribed)
                {
                    if (null != books)
                    {
                        callback.OnAudioBooksReady(books, Bundle.Empty);
                    }

                    return;
                }

                subscribed = true;
                
                var mediaId = mediaBrowser.Root;
                
                mediaBrowser.Subscribe(mediaId, Bundle.Empty, subscriptionCallbacks);
            }

            public void UpdateLibrary(IUpdateCallback callback)
            {
                var extras = new Bundle();

                extras.PutInt("Test1", 1);
                extras.PutString("Test2", "Lorem Ipsum");

                mediaBrowser.SendCustomAction(
                    MediaBrowserService.MediaBrowserService.IMediaLibraryActions.Update,
                    extras,
                    new CustomActionCallback<IUpdateCallback>(callback)
                    {
                        OnResultImpl = DoUpdateLibraryResult,
                        OnProgressUpdateImpl = DoUpdateLibraryProgress
                        // OnErrorImpl = 
                    }
                );
            }

            public void PrepareFromMediaId(string mediaId, Bundle extra)
            {
                var transport = mediaController.GetTransportControls();
                transport.PrepareFromMediaId(mediaId, extra);
            }

            public void GetHistory(IHistoryCallback callback)
            {
                var extras = new Bundle();

                extras.PutInt("Test1", 1);
                extras.PutString("Test2", "Lorem Ipsum");

                mediaBrowser.SendCustomAction(
                    MediaBrowserService.MediaBrowserService.IMediaLibraryActions.GetHistory,
                    extras,
                    new CustomActionCallback<IHistoryCallback>(callback)
                    {
                        OnResultImpl = DoGetHistoryResult,
                        //OnProgressUpdateImpl = DoUpdateLibraryProgress
                        OnErrorImpl = DoGetHistoryError
                    }
                );
            }

            public IDisposable Subscribe(IMediaServiceObserver observer)
            {
                var subscription = subscriptions.Find(x => ReferenceEquals(x.Observer, observer));

                if (null == subscription)
                {
                    subscription = new Subscription(observer);
                    subscriptions.Add(subscription);
                }

                return subscription;
            }

            private void DoUpdateLibraryResult(IUpdateCallback callback, string action, Bundle options, Bundle result)
            {
                System.Diagnostics.Debug.WriteLine("[DoUpdateLibraryResult] Execute");

                callback.OnUpdateResult();

                /*var callbacks = booksCallbacks.ToArray();

                for (var index = 0; index < callbacks.Length; index++)
                {
                    var callback = callbacks[index];
                    callback.OnAudioBooksReady(books, result);
                }*/
            }

            private void DoUpdateLibraryProgress(IUpdateCallback callback, string action, Bundle options, Bundle progress)
            {
                //var test1 = options.GetInt("Test1");
                var step = progress.GetInt("Update.Step");
                var percentage = progress.GetFloat("Update.Progress");

                callback.OnUpdateProgress(step, percentage);
            }

            private void DoGetHistoryResult(IHistoryCallback callback, string action, Bundle options, Bundle result)
            {
                var list = new List<MediaBrowserCompat.MediaItem>();

                callback.OnHistoryReady(list, options);
            }

            private void DoGetHistoryError(IHistoryCallback callback, string action, Bundle options, Bundle result)
            {
                callback.OnHistoryError(options);
            }

            private void DoOnChildrenLoaded(string arg1, IList<MediaBrowserCompat.MediaItem> arg2, Bundle arg3)
            {
                System.Diagnostics.Debug.WriteLine("[DoOnChildrenLoaded] Execute");

                books = arg2;

                var callbacks = booksCallbacks.ToArray();

                for (var index = 0; index < callbacks.Length; index++)
                {
                    var callback = callbacks[index];
                    callback.OnAudioBooksReady(arg2, arg3);
                }
            }

            private void DoOnError(string arg1, Bundle arg2)
            {
                var callbacks = booksCallbacks.ToArray();

                for (var index = 0; index < callbacks.Length; index++)
                {
                    var callback = callbacks[index];
                    callback.OnAudioBooksError(arg2);
                }
            }

            private void DoMetadataChanged(MediaMetadataCompat mediaMetadata)
            {
                var handlers = subscriptions.ToArray();

                for (var index = 0; index < handlers.Length; index++)
                {
                    var observer = handlers[index].Observer;
                    observer.OnMetadataChanged(mediaMetadata);
                }
            }

            private void DoQueueTitleChanged(string title)
            {
                var handlers = subscriptions.ToArray();

                for (var index = 0; index < handlers.Length; index++)
                {
                    var observer = handlers[index].Observer;
                    observer.OnQueueTitleChanged(title);
                }
            }

            private void DoQueueChanged(IList<MediaSessionCompat.QueueItem> queue)
            {
                var handlers = subscriptions.ToArray();

                for (var index = 0; index < handlers.Length; index++)
                {
                    var observer = handlers[index].Observer;
                    observer.OnQueueChanged(queue);
                }
            }

            private sealed class Subscription : IDisposable
            {
                public IMediaServiceObserver Observer
                {
                    get;
                }

                public Subscription(IMediaServiceObserver observer)
                {
                    Observer = observer;
                }

                public void Dispose()
                {
                    throw new NotImplementedException();
                }
            }
        }
    }
}