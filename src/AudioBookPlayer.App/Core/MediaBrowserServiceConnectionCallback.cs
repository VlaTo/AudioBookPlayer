using System;
using Android.Support.V4.Media;

namespace AudioBookPlayer.App.Core
{
    internal sealed class MediaBrowserServiceConnectionCallback : MediaBrowserCompat.ConnectionCallback
    {
        public Action OnConnectedImpl
        {
            get; 
            set;
        }

        public Action OnConnectionFailedImpl
        {
            get; 
            set;
        }

        public Action OnConnectionSuspendedImpl
        {
            get; 
            set;
        }

        public MediaBrowserServiceConnectionCallback()
        {
            OnConnectedImpl = Stub.Nop();
            OnConnectionFailedImpl = Stub.Nop();
            OnConnectionSuspendedImpl = Stub.Nop();
        }

        public override void OnConnected() => OnConnectedImpl.Invoke();

        public override void OnConnectionFailed() => OnConnectionFailedImpl.Invoke();

        public override void OnConnectionSuspended() => OnConnectionSuspendedImpl.Invoke();
    }
}