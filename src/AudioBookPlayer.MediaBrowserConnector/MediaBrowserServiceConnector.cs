using System;
using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V4.Media;
using AudioBookPlayer.Core;
using Java.Lang;

#nullable enable

namespace AudioBookPlayer.MediaBrowserConnector
{
    public sealed partial class MediaBrowserServiceConnector : Java.Lang.Object
    {
        private static MediaBrowserServiceConnector? instance;

        private readonly Context context;
        private ConnectionState state;
        private bool disposed;
        private MediaService? mediaBrowserService;
        private readonly MediaBrowserCompat mediaBrowser;
        private readonly List<IConnectCallback> connectCallbacks;

        public static MediaBrowserServiceConnector GetInstance()
        {
            if (null == instance)
            {
                var serviceName = Class.FromType(typeof(MediaBrowserService.MediaBrowserService));
                var componentName = new ComponentName(Application.Context, serviceName);

                instance = new MediaBrowserServiceConnector(Application.Context, componentName);
            }

            return instance;
        }

        /// <summary>
        /// The <see cref="IConnectCallback" /> interface.
        /// </summary>
        public interface IConnectCallback
        {
            void OnConnected(MediaService service);

            void OnSuspended();

            void OnFailed();
        }
        
        private MediaBrowserServiceConnector(Context context, ComponentName componentName)
        {
            var connectionCallback = new ConnectionCallback
            {
                OnConnectedImpl = DoConnected,
                OnConnectionSuspendedImpl = DoConnectionSuspended,
                OnConnectionFailedImpl = DoConnectionFailed
            };

            this.context = context;

            state = ConnectionState.NotConnected;
            mediaBrowserService = null;
            mediaBrowser = new MediaBrowserCompat(context, componentName, connectionCallback, Bundle.Empty);
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
            mediaBrowserService = new MediaService(context, mediaBrowser);
            
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

        #region ConnectionState

        private enum ConnectionState
        {
            Failed = -1,
            NotConnected,
            Connecting,
            Connected,
            Suspended
        }

        #endregion

        #region ConnectionCallback

        private sealed class ConnectionCallback : MediaBrowserCompat.ConnectionCallback
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

            public ConnectionCallback()
            {
                OnConnectedImpl = Stub.Nop();
                OnConnectionFailedImpl = Stub.Nop();
                OnConnectionSuspendedImpl = Stub.Nop();
            }

            public override void OnConnected() => OnConnectedImpl.Invoke();

            public override void OnConnectionFailed() => OnConnectionFailedImpl.Invoke();

            public override void OnConnectionSuspended() => OnConnectionSuspendedImpl.Invoke();
        }

        #endregion

    }
}

#nullable restore