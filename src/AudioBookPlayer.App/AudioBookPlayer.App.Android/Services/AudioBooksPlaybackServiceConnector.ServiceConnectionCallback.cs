using System;
using Android.Support.V4.Media;

namespace AudioBookPlayer.App.Android.Services
{
    internal sealed partial class AudioBooksPlaybackServiceConnector
    {
        private sealed class ServiceConnectionCallback : MediaBrowserCompat.ConnectionCallback
        {
            public Action OnConnectImpl
            {
                get;
                set;
            }

            public Action OnConnectionSuspendedImpl
            {
                get;
                set;
            }

            public Action OnConnectionFailedImpl
            {
                get;
                set;
            }

            public override void OnConnected() => OnConnectImpl.Invoke();

            public override void OnConnectionSuspended() => OnConnectionSuspendedImpl.Invoke();

            public override void OnConnectionFailed() => OnConnectionFailedImpl.Invoke();
        }
    }
}