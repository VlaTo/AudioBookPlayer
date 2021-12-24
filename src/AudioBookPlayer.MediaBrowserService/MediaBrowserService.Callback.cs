﻿using System;
using Android.OS;
using Android.Support.V4.Media.Session;
using AudioBookPlayer.MediaBrowserService.Core.Internal;

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

            public Callback()
            {
                OnPrepareFromMediaIdImpl = Stub.Nop<string, Bundle>();
            }

            public override void OnPrepareFromMediaId(string mediaId, Bundle extras) =>
                OnPrepareFromMediaIdImpl.Invoke(mediaId, extras);
        }
    }
}