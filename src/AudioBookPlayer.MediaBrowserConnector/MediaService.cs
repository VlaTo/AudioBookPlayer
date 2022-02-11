using Android.Content;
using Android.OS;
using Android.Support.V4.Media;
using Android.Support.V4.Media.Session;
using AudioBookPlayer.Core;
using Java.Lang;
using System;
using System.Collections.Generic;
using Debug = System.Diagnostics.Debug;

namespace AudioBookPlayer.MediaBrowserConnector
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class MediaService
    {
        private readonly MediaBrowserCompat mediaBrowser;
        private readonly SubscriptionCallback mediaBrowserCallback;
        private readonly List<IAudioBooksListener> booksListeners;
        private readonly List<IMediaServiceListener> listeners;
        private readonly MediaControllerCompat mediaController;
        private IList<MediaBrowserCompat.MediaItem> books;
        private bool subscribed;

        #region IMediaServiceListener

        /// <summary>
        /// 
        /// </summary>
        public interface IMediaServiceListener
        {
            void OnMetadataChanged(MediaMetadataCompat metadata);

            void OnQueueTitleChanged(string title);

            void OnQueueChanged(IList<MediaSessionCompat.QueueItem> queue);
        }

        #endregion

        #region IAudioBooksListener

        /// <summary>
        /// 
        /// </summary>
        public interface IAudioBooksListener
        {
            void OnReady(IList<MediaBrowserCompat.MediaItem> list, Bundle options);

            void OnError(Bundle options);
        }

        #endregion

        #region IUpdateListener

        public interface IUpdateListener
        {
            void OnUpdateProgress(int step, float progress);

            void OnUpdateResult();
        }

        #endregion
        
        public MediaService(Context context, MediaBrowserCompat mediaBrowser)
        {
            this.mediaBrowser = mediaBrowser;

            subscribed = false;
            books = null;
            booksListeners = new List<IAudioBooksListener>();
            listeners = new List<IMediaServiceListener>();
            mediaBrowserCallback = new SubscriptionCallback
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

        public void GetAudioBooks(IAudioBooksListener listener)
        {
            if (false == booksListeners.Contains(listener))
            {
                booksListeners.Add(listener);
            }

            if (subscribed)
            {
                if (null != books)
                {
                    listener.OnReady(books, Bundle.Empty);
                }

                return;
            }

            subscribed = true;
                
            var mediaId = mediaBrowser.Root;
                
            mediaBrowser.Subscribe(mediaId, Bundle.Empty, mediaBrowserCallback);
        }

        public void UpdateLibrary(IUpdateListener listener)
        {
            var extras = new Bundle();

            extras.PutInt("Test1", 1);
            extras.PutString("Test2", "Lorem Ipsum");

            mediaBrowser.SendCustomAction(
                MediaBrowserService.MediaBrowserService.IMediaLibraryActions.Update,
                extras,
                new CustomActionCallback<IUpdateListener>(listener)
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

        /*public void GetHistory(MediaBrowserServiceConnector.IHistoryCallback callback)
        {
            var extras = new Bundle();

            extras.PutInt("Test1", 1);
            extras.PutString("Test2", "Lorem Ipsum");

            mediaBrowser.SendCustomAction(
                MediaBrowserService.MediaBrowserService.IMediaLibraryActions.GetHistory,
                extras,
                new CustomActionCallback<MediaBrowserServiceConnector.IHistoryCallback>(callback)
                {
                    OnResultImpl = DoGetHistoryResult,
                    //OnProgressUpdateImpl = DoUpdateLibraryProgress
                    OnErrorImpl = DoGetHistoryError
                }
            );
        }*/

        public void AddListener(IMediaServiceListener listener)
        {
            if (listeners.Contains(listener))
            {
                return;
            }

            listeners.Add(listener);
        }

        public void RemoveListener(IMediaServiceListener listener)
        {
            if (listeners.Remove(listener))
            {
                ;
            }
        }

        #region MediaService.IUpdateCallback listeners

        private static void DoUpdateLibraryResult(IUpdateListener listener, string action, Bundle options, Bundle result)
        {
            listener.OnUpdateResult();
        }

        private static void DoUpdateLibraryProgress(IUpdateListener listener, string action, Bundle options, Bundle progress)
        {
            //var test1 = options.GetInt("Test1");
            var step = progress.GetInt("Update.Step");
            var percentage = progress.GetFloat("Update.Progress");

            listener.OnUpdateProgress(step, percentage);
        }

        #endregion

        private void DoGetHistoryResult(MediaBrowserServiceConnector.IHistoryCallback callback, string action, Bundle options, Bundle result)
        {
            var list = new List<MediaBrowserCompat.MediaItem>();

            callback.OnHistoryReady(list, options);
        }

        private void DoGetHistoryError(MediaBrowserServiceConnector.IHistoryCallback callback, string action, Bundle options, Bundle result)
        {
            callback.OnHistoryError(options);
        }

        private void DoOnChildrenLoaded(string arg1, IList<MediaBrowserCompat.MediaItem> arg2, Bundle arg3)
        {
            Debug.WriteLine("[DoOnChildrenLoaded] Execute");

            books = arg2;

            var callbacks = booksListeners.ToArray();

            for (var index = 0; index < callbacks.Length; index++)
            {
                var callback = callbacks[index];
                callback.OnReady(arg2, arg3);
            }
        }

        private void DoOnError(string arg1, Bundle arg2)
        {
            var callbacks = booksListeners.ToArray();

            for (var index = 0; index < callbacks.Length; index++)
            {
                var callback = callbacks[index];
                callback.OnError(arg2);
            }
        }

        #region MediaServiceListeners

        private void DoMetadataChanged(MediaMetadataCompat mediaMetadata)
        {
            var handlers = listeners.ToArray();

            for (var index = 0; index < handlers.Length; index++)
            {
                handlers[index].OnMetadataChanged(mediaMetadata);
            }
        }

        private void DoQueueTitleChanged(string title)
        {
            var handlers = listeners.ToArray();

            for (var index = 0; index < handlers.Length; index++)
            {
                handlers[index].OnQueueTitleChanged(title);
            }
        }

        private void DoQueueChanged(IList<MediaSessionCompat.QueueItem> queue)
        {
            var handlers = listeners.ToArray();

            for (var index = 0; index < handlers.Length; index++)
            {
                handlers[index].OnQueueChanged(queue);
            }
        }

        #endregion

        #region SubscriptionCallback

        private sealed class SubscriptionCallback : MediaBrowserCompat.SubscriptionCallback
        {
            public Action<string, IList<MediaBrowserCompat.MediaItem>, Bundle> OnChildrenLoadedImpl
            {
                get; 
                set;
            }

            public Action<string, Bundle> OnErrorImpl
            {
                get; 
                set;
            }

            public SubscriptionCallback()
            {
                OnChildrenLoadedImpl = Stub.Nop<string, IList<MediaBrowserCompat.MediaItem>, Bundle>();
                OnErrorImpl = Stub.Nop<string, Bundle>();
            }

            public override void OnChildrenLoaded(string parentId, IList<MediaBrowserCompat.MediaItem> children) =>
                OnChildrenLoadedImpl.Invoke(parentId, children, Bundle.Empty);

            public override void OnChildrenLoaded(string parentId, IList<MediaBrowserCompat.MediaItem> children,
                Bundle options) =>
                OnChildrenLoadedImpl.Invoke(parentId, children, options);

            public override void OnError(string parentId) => OnErrorImpl.Invoke(parentId, Bundle.Empty);

            public override void OnError(string parentId, Bundle options) => OnErrorImpl.Invoke(parentId, options);
        }

        #endregion

        #region MediaControllerCallback

        private sealed class MediaControllerCallback : MediaControllerCompat.Callback
        {
            public Action<PlaybackStateCompat> OnPlaybackStateChangedImpl
            {
                get; 
                set;
            }

            public Action<MediaMetadataCompat> OnMetadataChangedImpl
            {
                get; 
                set;
            }

            public Action<IList<MediaSessionCompat.QueueItem>> OnQueueChangedImpl
            {
                get; 
                set;
            }

            public Action<string> OnQueueTitleChangedImpl
            {
                get; 
                set;
            }

            public Action<MediaControllerCompat.PlaybackInfo> OnAudioInfoChangedImpl
            {
                get; 
                set;
            }

            public Action<Bundle> OnExtrasChangedImpl
            {
                get; 
                set;
            }

            public MediaControllerCallback()
            {
                OnPlaybackStateChangedImpl = Stub.Nop<PlaybackStateCompat>();
                OnMetadataChangedImpl = Stub.Nop<MediaMetadataCompat>();
                OnQueueChangedImpl = Stub.Nop<IList<MediaSessionCompat.QueueItem>>();
                OnQueueTitleChangedImpl = Stub.Nop<string>();
                OnAudioInfoChangedImpl = Stub.Nop<MediaControllerCompat.PlaybackInfo>();
                OnExtrasChangedImpl = Stub.Nop<Bundle>();
            }

            public override void OnAudioInfoChanged(MediaControllerCompat.PlaybackInfo info) =>
                OnAudioInfoChangedImpl.Invoke(info);

            public override void OnCaptioningEnabledChanged(bool enabled)
            {
                base.OnCaptioningEnabledChanged(enabled);
            }

            public override void OnExtrasChanged(Bundle extras) => OnExtrasChangedImpl.Invoke(extras);

            public override void OnMetadataChanged(MediaMetadataCompat metadata) =>
                OnMetadataChangedImpl.Invoke(metadata);

            public override void OnPlaybackStateChanged(PlaybackStateCompat state) =>
                OnPlaybackStateChangedImpl.Invoke(state);

            public override void OnQueueChanged(IList<MediaSessionCompat.QueueItem> queue) =>
                OnQueueChangedImpl.Invoke(queue);

            public override void OnQueueTitleChanged(ICharSequence title) =>
                OnQueueTitleChangedImpl.Invoke(title.ToString());

            public override void OnRepeatModeChanged(int repeatMode)
            {
                base.OnRepeatModeChanged(repeatMode);
            }

            public override void OnSessionDestroyed()
            {
                base.OnSessionDestroyed();
            }

            public override void OnSessionEvent(string e, Bundle extras)
            {
                base.OnSessionEvent(e, extras);
            }

            public override void OnSessionReady()
            {
                base.OnSessionReady();
            }

            public override void OnShuffleModeChanged(int shuffleMode)
            {
                base.OnShuffleModeChanged(shuffleMode);
            }
        }

        #endregion

        #region CustomActionCallback

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
                OnErrorImpl = Stub.Nop<string, Bundle, Bundle>();
                OnProgressUpdateImpl = Stub.Nop<string, Bundle, Bundle>();
                OnResultImpl = Stub.Nop<string, Bundle, Bundle>();
            }

            public override void OnError(string action, Bundle extras, Bundle data) =>
                OnErrorImpl.Invoke(action, extras, data);

            public override void OnProgressUpdate(string action, Bundle extras, Bundle data) =>
                OnProgressUpdateImpl.Invoke(action, extras, data);

            public override void OnResult(string action, Bundle extras, Bundle result) =>
                OnResultImpl.Invoke(action, extras, result);
        }

        private sealed class CustomActionCallback<T> : MediaBrowserCompat.CustomActionCallback
        {
            public T Arg
            {
                get;
            }

            public Action<T, string, Bundle, Bundle> OnErrorImpl
            {
                get;
                set;
            }

            public Action<T, string, Bundle, Bundle> OnProgressUpdateImpl
            {
                get;
                set;
            }

            public Action<T, string, Bundle, Bundle> OnResultImpl
            {
                get;
                set;
            }

            public CustomActionCallback(T arg)
            {
                Arg = arg;
                OnErrorImpl = Stub.Nop<T, string, Bundle, Bundle>();
                OnProgressUpdateImpl = Stub.Nop<T, string, Bundle, Bundle>();
                OnResultImpl = Stub.Nop<T, string, Bundle, Bundle>();
            }

            public override void OnError(string action, Bundle extras, Bundle data) =>
                OnErrorImpl.Invoke(Arg, action, extras, data);

            public override void OnProgressUpdate(string action, Bundle extras, Bundle data) =>
                OnProgressUpdateImpl.Invoke(Arg, action, extras, data);

            public override void OnResult(string action, Bundle extras, Bundle result) =>
                OnResultImpl.Invoke(Arg, action, extras, result);
        }

        #endregion
    }
}