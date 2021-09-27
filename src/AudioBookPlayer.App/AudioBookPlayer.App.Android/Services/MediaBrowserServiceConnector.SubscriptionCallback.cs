using System;
using System.Collections.Generic;
using Android.OS;
using Android.Support.V4.Media;
using LibraProgramming.Xamarin.Core;

namespace AudioBookPlayer.App.Android.Services
{
    internal partial class MediaBrowserServiceConnector
    {
        /// <summary>
        /// The Subscription Callback class.
        /// </summary>
        private sealed class SubscriptionCallback : MediaBrowserCompat.SubscriptionCallback
        {
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

            public SubscriptionCallback()
            {
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
    }
}