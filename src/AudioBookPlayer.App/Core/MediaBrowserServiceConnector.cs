#nullable enable

using Android.Content;
using Android.OS;
using Android.Support.V4.Media;
using Java.Lang;
using System;
using System.Collections.Generic;
using Object = Java.Lang.Object;

namespace AudioBookPlayer.App.Core
{
    internal sealed partial class MediaBrowserServiceConnector : Object
    {
        private readonly Context context;
        private ConnectionState state;
        private bool disposed;
        private MediaBrowserServiceImpl? mediaBrowserService;
        private readonly MediaBrowserCompat mediaBrowser;
        private readonly List<IConnectCallback> connectCallbacks;

        public MediaBrowserServiceConnector(Context context)
        {
            var serviceName = Class.FromType(typeof(MediaBrowserService.MediaBrowserService));
            var componentName = new ComponentName(context, serviceName);
            var callbacks = new MediaBrowserServiceConnectionCallback
            {
                OnConnectedImpl = DoConnected,
                OnConnectionSuspendedImpl = DoConnectionSuspended,
                OnConnectionFailedImpl = DoConnectionFailed
            };

            this.context = context;

            state = ConnectionState.NotConnected;
            mediaBrowserService = null;
            mediaBrowser = new MediaBrowserCompat(context, componentName, callbacks, Bundle.Empty);
            connectCallbacks = new List<IConnectCallback>();
        }

        public void Connect(IConnectCallback callback)
        {
            EnsureNotDisposed();

            if (null == callback)
            {
                throw new ArgumentNullException(nameof(callback));
            }

            switch (state)
            {
                case ConnectionState.NotConnected:
                {
                    if (false == connectCallbacks.Contains(callback))
                    {
                        connectCallbacks.Add(callback);
                    }

                    state = ConnectionState.Connecting;
                    mediaBrowser.Connect();

                    break;
                }

                case ConnectionState.Connecting:
                {
                    if (false == connectCallbacks.Contains(callback))
                    {
                        connectCallbacks.Add(callback);
                    }

                    break;
                }

                case ConnectionState.Connected:
                {
                    callback.OnConnected(mediaBrowserService);

                    break;
                }

                case ConnectionState.Failed:
                {
                    callback.OnFailed();

                    break;
                }

                case ConnectionState.Suspended:
                {
                    callback.OnSuspended();

                    break;
                }
            }
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            try
            {
                if (disposing)
                {
                    DoDispose();
                }
            }
            finally
            {
                disposed = true;
            }
        }

        private void DoConnected()
        {
            EnsureNotDisposed();

            state = ConnectionState.Connected;
            mediaBrowserService = new MediaBrowserServiceImpl(context, mediaBrowser);
            
            var callbacks = connectCallbacks.ToArray();

            for (var index = 0; index < callbacks.Length; index++)
            {
                var callback = callbacks[index];
                callback.OnConnected(mediaBrowserService);
            }
        }

        private void DoConnectionSuspended()
        {
            EnsureNotDisposed();

            state = ConnectionState.Suspended;
            //mediaBrowserService = new MediaBrowserServiceImpl(context, mediaBrowser.SessionToken);

            var callbacks = connectCallbacks.ToArray();

            for (var index = 0; index < callbacks.Length; index++)
            {
                var callback = callbacks[index];
                callback.OnSuspended();
            }
        }

        private void DoConnectionFailed()
        {
            EnsureNotDisposed();

            state = ConnectionState.Failed;
            mediaBrowserService = null;

            var callbacks = connectCallbacks.ToArray();

            for (var index = 0; index < callbacks.Length; index++)
            {
                var callback = callbacks[index];
                callback.OnFailed();
            }
        }

        private void DoDispose()
        {
            ;
        }

        private void EnsureNotDisposed()
        {
            if (disposed)
            {
                throw new ObjectDisposedException(nameof(MediaBrowserServiceConnector));
            }
        }

        // Enum
        private enum ConnectionState
        {
            Failed = -1,
            NotConnected,
            Connecting,
            Connected,
            Suspended
        }
    }
}