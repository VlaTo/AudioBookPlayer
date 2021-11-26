#nullable enable

using Android.Content;
using Android.OS;
using Android.Support.V4.Media;
using Android.Support.V4.Media.Session;
using AudioBookPlayer.Domain;
using Java.Lang;
using System.Collections.Generic;

namespace AudioBookPlayer.App.Core
{
    internal sealed partial class MediaBrowserServiceConnector : Object, MediaBrowserServiceConnector.IConnectCallback
    {
        private readonly Context context;
        private readonly MediaControllerCallback mediaControllerCallback;
        private readonly ConnectExecutor connectExecutor;
        private bool disposed;
        private MediaControllerCompat? mediaController;

        public bool IsConnected => ConnectExecutor.ConnectState.Connected == connectExecutor.State;

        public MediaControllerCompat? MediaController
        {
            get => mediaController;
            private set
            {
                if (ReferenceEquals(value, mediaController))
                {
                    return;
                }

                if (null != mediaController)
                {
                    mediaController.UnregisterCallback(mediaControllerCallback);
                }

                mediaController = value;

                if (null != mediaController)
                {
                    mediaController.RegisterCallback(mediaControllerCallback);
                }
            }
        }

        public MediaSessionCompat.Token SessionToken => connectExecutor.SessionToken;

        public MediaBrowserServiceConnector(Context context)
        {
            this.context = context;
            connectExecutor = new ConnectExecutor(context, Bundle.Empty, this);
            mediaControllerCallback = new MediaControllerCallback
            {
                OnMetadataChangedImpl = DoMetadataChanged,
                OnPlaybackStateChangedImpl = DoPlaybackStateChanged,
                OnQueueChangedImpl = DoQueueChanged,
                OnQueueTitleChangedImpl = DoQueueTitleChanged
            };
        }

        public void Connect(IConnectCallback callback) => connectExecutor.Connect(callback);

        public void GetAudioBooks(IAudioBooksResultCallback callback) => connectExecutor.GetAudioBooks(callback);

        void IConnectCallback.OnConnected()
        {
            MediaController = new MediaControllerCompat(context, connectExecutor.SessionToken);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            try
            {
                if (disposing)
                {
                    connectExecutor.Disconnect();
                }
            }
            finally
            {
                disposed = true;
            }
        }

        private void DoMetadataChanged(MediaMetadataCompat obj)
        {
            throw new System.NotImplementedException();
        }

        private void DoPlaybackStateChanged(PlaybackStateCompat obj)
        {
            throw new System.NotImplementedException();
        }

        private void DoQueueChanged(IList<MediaSessionCompat.QueueItem> obj)
        {
            throw new System.NotImplementedException();
        }

        private void DoQueueTitleChanged(ICharSequence obj)
        {
            throw new System.NotImplementedException();
        }

        //

        public interface IConnectCallback
        {
            void OnConnected();
        }

        //
        public interface IAudioBooksResultCallback
        {
            void OnAudioBooksResult(IReadOnlyList<AudioBookDescription> list);

            void OnAudioBooksError();
        }
    }
}