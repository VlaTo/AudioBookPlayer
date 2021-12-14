using Android.Content;
using Android.OS;
using Android.Support.V4.Media;
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

        /// <summary>
        /// 
        /// </summary>
        internal interface IMediaBrowserService
        {
            void GetAudioBooks(IAudioBooksCallback callback);
        }

        /// <summary>
        /// 
        /// </summary>
        private sealed class MediaBrowserServiceImpl : IMediaBrowserService
        {
            private readonly Context context;
            private readonly MediaBrowserCompat mediaBrowser;
            private readonly MediaBrowserSubscriptionCallback subscriptionCallbacks;
            private readonly List<IAudioBooksCallback> booksCallbacks;
            private IList<MediaBrowserCompat.MediaItem> books;
            private bool subscribed;

            public MediaBrowserServiceImpl(Context context, MediaBrowserCompat mediaBrowser)
            {
                this.context = context;
                this.mediaBrowser = mediaBrowser;

                subscribed = false;
                books = null;
                booksCallbacks = new List<IAudioBooksCallback>();
                subscriptionCallbacks = new MediaBrowserSubscriptionCallback
                {
                    OnChildrenLoadedImpl = DoOnChildrenLoaded,
                    OnErrorImpl = DoOnError
                };

                /*var callbacks = new MediaControllerCallback
                {
                    OnMetadataChangedImpl = DoMetadataChanged,
                    OnPlaybackStateChangedImpl = DoPlaybackStateChanged,
                    OnQueueChangedImpl = DoQueueChanged,
                    OnQueueTitleChangedImpl = DoQueueTitleChanged
                };

                var mediaController = new MediaControllerCompat(context, mediaBrowser.SessionToken);
                mediaController.RegisterCallback(callbacks);*/
            }

            private void DoOnChildrenLoaded(string arg1, IList<MediaBrowserCompat.MediaItem> arg2, Bundle arg3)
            {
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
        }
    }
}