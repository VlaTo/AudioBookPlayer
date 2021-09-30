﻿#nullable enable

using System;
using Android.Content;

namespace AudioBookPlayer.App.Android.Core
{
    internal partial class Playback
    {
        /// <summary>
        /// 
        /// </summary>
        private sealed class BroadcastReceiver : global::Android.Content.BroadcastReceiver
        {
            public Action<Context?, Intent?> OnReceiveImpl
            {
                get;
                set;
            }

            public BroadcastReceiver()
            {
                ;
            }

            public override void OnReceive(Context? context, Intent? intent) => OnReceiveImpl.Invoke(context, intent);
        }
    }
}