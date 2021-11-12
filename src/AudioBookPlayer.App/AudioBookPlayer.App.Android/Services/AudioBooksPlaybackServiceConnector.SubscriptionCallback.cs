using Android.OS;
using Android.Support.V4.Media;
using LibraProgramming.Xamarin.Core;
using System;
using System.Collections.Generic;

namespace AudioBookPlayer.App.Android.Services
{
    internal partial class AudioBooksPlaybackServiceConnector
    {
        /// <summary>
        /// The Subscription Callback class.
        /// </summary>
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

            public override void OnChildrenLoaded(
                string parentId,
                IList<MediaBrowserCompat.MediaItem> children
            ) =>
                OnChildrenLoadedImpl.Invoke(parentId, children, Bundle.Empty);

            public override void OnChildrenLoaded(
                string parentId,
                IList<MediaBrowserCompat.MediaItem> children,
                Bundle options
            ) =>
                OnChildrenLoadedImpl?.Invoke(parentId, children, options);

            public override void OnError(string parentId) => OnErrorImpl.Invoke(parentId, Bundle.Empty);

            public override void OnError(string parentId, Bundle options) => OnErrorImpl.Invoke(parentId, options);
        }
    }
}