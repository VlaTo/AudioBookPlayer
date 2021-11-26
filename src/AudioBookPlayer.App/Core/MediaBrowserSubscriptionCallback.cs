using System;
using System.Collections.Generic;
using Android.OS;
using Android.Support.V4.Media;

namespace AudioBookPlayer.App.Core
{
    internal sealed class MediaBrowserSubscriptionCallback : MediaBrowserCompat.SubscriptionCallback
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

        public MediaBrowserSubscriptionCallback()
        {
            OnChildrenLoadedImpl = Stub.Nop<string, IList<MediaBrowserCompat.MediaItem>, Bundle>();
            OnErrorImpl = Stub.Nop<string, Bundle>();
        }

        public override void OnChildrenLoaded(string parentId, IList<MediaBrowserCompat.MediaItem> children) =>
            OnChildrenLoadedImpl.Invoke(parentId, children, Bundle.Empty);

        public override void OnChildrenLoaded(string parentId, IList<MediaBrowserCompat.MediaItem> children, Bundle options) =>
            OnChildrenLoadedImpl.Invoke(parentId, children, options);

        public override void OnError(string parentId) => OnErrorImpl.Invoke(parentId, Bundle.Empty);

        public override void OnError(string parentId, Bundle options) => OnErrorImpl.Invoke(parentId, options);
    }
}