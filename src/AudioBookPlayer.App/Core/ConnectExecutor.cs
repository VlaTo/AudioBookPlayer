using Android.Content;
using Android.OS;
using Android.Support.V4.Media;
using Android.Support.V4.Media.Session;
using AudioBookPlayer.Domain;
using Java.Lang;
using System.Collections.Generic;
using Android.App;

namespace AudioBookPlayer.App.Core
{
    /*
    internal sealed partial class ConnectExecutor
    {
        private readonly MediaBrowserServiceConnector.IConnectCallback owner;
        private readonly MediaBrowserCompat mediaBrowser;
        private readonly MediaBrowserSubscriptionCallback browserCallback;
        private readonly List<MediaBrowserServiceConnector.IConnectCallback> connectCallbacks;
        private readonly List<MediaBrowserServiceConnector.IAudioBooksCallback> childrenCallback;
        private readonly List<AudioBookDescription> descriptions;
        private AbstractConnectState state;

        public ConnectState State => state.State;

        public MediaSessionCompat.Token SessionToken => mediaBrowser.SessionToken;

        public ConnectExecutor(Context context, Bundle rootHints, MediaBrowserServiceConnector.IConnectCallback owner)
        {
            var serviceName = Class.FromType(typeof(MediaBrowserService.MediaBrowserService));
            var componentName = new ComponentName(context, serviceName);
            var connectionCallbacks = new MediaBrowserServiceConnectionCallback
            {
                OnConnectedImpl = DoConnected,
                OnConnectionSuspendedImpl = DoConnectionSuspended,
                OnConnectionFailedImpl = DoConnectionFailed
            };

            this.owner = owner;
            mediaBrowser = new MediaBrowserCompat(context, componentName, connectionCallbacks, rootHints);
            connectCallbacks = new List<MediaBrowserServiceConnector.IConnectCallback>();
            childrenCallback = new List<MediaBrowserServiceConnector.IAudioBooksCallback>();
            browserCallback = new MediaBrowserSubscriptionCallback
            {
                OnChildrenLoadedImpl = DoChildrenLoaded,
                OnErrorImpl = DoChildrenError
            };

            descriptions = new List<AudioBookDescription>();
            state = new IdleState(this);
        }

        public void Connect(MediaBrowserServiceConnector.IConnectCallback callback) => state.DoConnect(callback);

        public void Disconnect() => state.DoDisconnect();

        public void GetAudioBooks(MediaBrowserServiceConnector.IAudioBooksCallback callback) => state.DoGetAudioBooks(callback);

        public void UpdateLibrary() => state.DoUpdateLibrary();

        private void DoConnected() => state.DoConnected();

        private void DoConnectionSuspended()
        {
            ;
        }

        private void DoConnectionFailed() => state.DoConnectionFailed();

        private void DoChildrenError(string parentId, Bundle options)
        {
            var handlers = childrenCallback.ToArray();

            childrenCallback.Clear();

            for (var index = 0; index < handlers.Length; index++)
            {
                handlers[index].OnAudioBooksError();
            }
        }

        private void DoChildrenLoaded(string parentId, IList<MediaBrowserCompat.MediaItem> children, Bundle options)
        {
            for (var index = 0; index < children.Count; index++)
            {
                var description = AudioBookDescriptionBuilder.From(children[index]);
                descriptions.Add(description);
            }

            //
            var handlers = childrenCallback.ToArray();

            childrenCallback.Clear();

            for (var index = 0; index < handlers.Length; index++)
            {
                handlers[index].OnAudioBooksReady(descriptions);
            }
        }

        //
        public enum ConnectState
        {
            Error = -1,
            Idle,
            Connecting,
            Connected,
            Disconnected
        }

        //
        private abstract class AbstractConnectState
        {
            public abstract ConnectState State
            {
                get;
            }

            public abstract void DoConnect(MediaBrowserServiceConnector.IConnectCallback callback);

            public abstract void DoConnected();

            public abstract void DoConnectionFailed();

            public abstract void DoDisconnect();

            public abstract void DoGetAudioBooks(MediaBrowserServiceConnector.IAudioBooksCallback callback);

            public abstract void DoUpdateLibrary();
        }

        //
        private sealed class IdleState : AbstractConnectState
        {
            private readonly ConnectExecutor executor;

            public override ConnectState State { get; } = ConnectState.Idle;

            public IdleState(ConnectExecutor executor)
            {
                this.executor = executor;
            }

            public override void DoConnect(MediaBrowserServiceConnector.IConnectCallback callback)
            {
                executor.state = new ConnectingState(executor);
                executor.connectCallbacks.Add(callback);
                executor.mediaBrowser.Connect();
            }

            public override void DoConnected() => throw new System.NotImplementedException();

            public override void DoConnectionFailed() => throw new System.NotImplementedException();

            public override void DoDisconnect() => executor.state = new DisconnectedState(executor);
            
            public override void DoGetAudioBooks(MediaBrowserServiceConnector.IAudioBooksCallback callback)
            {
                executor.childrenCallback.Add(callback);
            }

            public override void DoUpdateLibrary()
            {
                throw new System.NotImplementedException();
            }
        }

        //
        private sealed class ConnectingState : AbstractConnectState
        {
            private readonly ConnectExecutor executor;
            
            public override ConnectState State { get; } = ConnectState.Connecting;

            public ConnectingState(ConnectExecutor executor)
            {
                this.executor = executor;
            }

            public override void DoConnect(MediaBrowserServiceConnector.IConnectCallback callback)
            {
                executor.connectCallbacks.Add(callback);
            }

            public override void DoConnected()
            {
                executor.state = new ConnectedState(executor);

                var handlers = executor.connectCallbacks.ToArray();

                executor.connectCallbacks.Clear();

                for (var index = 0; index < handlers.Length; index++)
                {
                    handlers[index].OnConnected();
                }

                executor.owner.OnConnected();

                if (0 < executor.childrenCallback.Count)
                {
                    var mediaId = executor.mediaBrowser.Root;
                    executor.mediaBrowser.Subscribe(mediaId, executor.browserCallback);
                }
            }

            public override void DoConnectionFailed()
            {
                executor.state = new FailedState(executor);
            }

            public override void DoDisconnect()
            {
                throw new System.NotImplementedException();
            }

            public override void DoGetAudioBooks(MediaBrowserServiceConnector.IAudioBooksCallback callback)
            {
                executor.childrenCallback.Add(callback);
            }

            public override void DoUpdateLibrary()
            {
                throw new System.NotImplementedException();
            }
        }

        //
        private sealed class ConnectedState : AbstractConnectState
        {
            private readonly ConnectExecutor executor;

            public override ConnectState State { get; } = ConnectState.Connected;

            public ConnectedState(ConnectExecutor executor)
            {
                this.executor = executor;
            }

            public override void DoConnect(MediaBrowserServiceConnector.IConnectCallback callback)
            {
                callback.OnConnected();
            }

            public override void DoDisconnect()
            {
                executor.state = new DisconnectedState(executor);
                executor.mediaBrowser.Disconnect();
            }

            public override void DoConnected() => throw new System.NotImplementedException();

            public override void DoConnectionFailed() => throw new System.NotImplementedException();

            public override void DoGetAudioBooks(MediaBrowserServiceConnector.IAudioBooksCallback callback)
            {
                callback.OnAudioBooksReady(executor.descriptions);
            }

            public override void DoUpdateLibrary()
            {
                var extras = new Bundle();
                var callback = new CustomActionCallback();
                executor.mediaBrowser.SendCustomAction(MediaBrowserService.MediaBrowserService.IMediaLibraryActions.Update, extras, callback);
            }
        }

        //
        private sealed class FailedState : AbstractConnectState
        {
            private readonly ConnectExecutor executor;

            public override ConnectState State { get; } = ConnectState.Error;

            public FailedState(ConnectExecutor executor)
            {
                this.executor = executor;
            }

            public override void DoConnect(MediaBrowserServiceConnector.IConnectCallback callback)
            {
                throw new System.NotImplementedException();
            }

            public override void DoConnected()
            {
                throw new System.NotImplementedException();
            }

            public override void DoConnectionFailed()
            {
                throw new System.NotImplementedException();
            }

            public override void DoDisconnect()
            {
                throw new System.NotImplementedException();
            }

            public override void DoGetAudioBooks(MediaBrowserServiceConnector.IAudioBooksCallback callback)
            {
                callback.OnAudioBooksError();
            }

            public override void DoUpdateLibrary()
            {
                throw new System.NotImplementedException();
            }
        }

        //
        private sealed class DisconnectedState : AbstractConnectState
        {
            private readonly ConnectExecutor executor;

            public override ConnectState State { get; } = ConnectState.Disconnected;

            public DisconnectedState(ConnectExecutor executor)
            {
                this.executor = executor;
            }

            public override void DoConnect(MediaBrowserServiceConnector.IConnectCallback callback)
            {
                throw new System.NotImplementedException();
            }

            public override void DoConnected()
            {
                throw new System.NotImplementedException();
            }

            public override void DoConnectionFailed()
            {
                throw new System.NotImplementedException();
            }

            public override void DoDisconnect()
            {
                throw new System.NotImplementedException();
            }

            public override void DoGetAudioBooks(MediaBrowserServiceConnector.IAudioBooksCallback callback)
            {
                throw new System.NotImplementedException();
            }

            public override void DoUpdateLibrary()
            {
                throw new System.NotImplementedException();
            }
        }
    }
*/
}