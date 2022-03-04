using Android.OS;
using Android.Support.V4.Media.Session;
using AudioBookPlayer.Core;
using System;

namespace AudioBookPlayer.MediaBrowserService
{
    public partial class MediaBrowserService
    {
        private sealed class Callback : MediaSessionCompat.Callback
        {
            public Action<string, Bundle> OnPrepareFromMediaIdImpl
            {
                get; 
                set;
            }

            public Action<long> OnSkipToQueueItemImpl
            {
                get; 
                set;
            }

            public Callback()
            {
                OnPrepareFromMediaIdImpl = Stub.Nop<string, Bundle>();
                OnSkipToQueueItemImpl = Stub.Nop<long>();
            }

            public override void OnPrepareFromMediaId(string mediaId, Bundle extras) =>
                OnPrepareFromMediaIdImpl.Invoke(mediaId, extras);

            public override void OnSkipToQueueItem(long id) => OnSkipToQueueItemImpl.Invoke(id);
        }
    }
}