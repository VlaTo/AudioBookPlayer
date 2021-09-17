using Android.Content;
using Android.OS;
using Android.Support.V4.Media;
using Android.Support.V4.Media.Session;
using AudioBookPlayer.App.Android.Services;
using AudioBookPlayer.App.Android.Services.Builders;
using AudioBookPlayer.App.Domain.Models;
using AudioBookPlayer.App.Services;
using System;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Xamarin.Forms;
using Application = Android.App.Application;

[assembly: Dependency(typeof(MediaBrowserServiceConnector))]

namespace AudioBookPlayer.App.Android.Services
{
    internal sealed class MediaBrowserServiceConnector : Java.Lang.Object, IMediaBrowserServiceConnector
    {
        private readonly MediaBrowserCompat mediaBrowser;
        //private readonly ConnectionCallback connectionCallback;
        //private readonly SubscriptionCallback subscriptionCallback;

        public bool IsConnected => mediaBrowser.IsConnected;

        public MediaSessionCompat.Token SessionToken => mediaBrowser.SessionToken;

        public IObservable<Unit> Connected
        {
            get;
        }

        public IObservable<AudioBook[]> Library
        {
            get;
        }

        public MediaBrowserServiceConnector()
        {
            var serviceName = Java.Lang.Class.FromType(typeof(AudioBookMediaBrowserService)).Name;
            var componentName = new ComponentName(Application.Context, serviceName);
            
            var connected = new Subject<Unit>();
            var library = new Subject<AudioBook[]>();
            var connectionCallback = new ConnectionCallback(this, connected, library);
            var publishConnected = connected.Take(1).Publish();
            var libraryConnected = library.Publish();
            
            publishConnected.Connect();
            libraryConnected.Connect();

            Connected = publishConnected.AsObservable();
            Library = libraryConnected.AsObservable();

            mediaBrowser = new MediaBrowserCompat(Application.Context, componentName, connectionCallback, null);
        }

        public void Connect()
        {
            if (mediaBrowser.IsConnected)
            {
                return;
            }

            mediaBrowser.Connect();
        }

        /*public IObservable<AudioBook[]> GetLibrary()
        {
            if (false==mediaBrowser.IsConnected)
            {
                return Observable.Empty<AudioBook[]>();
            }

            return Observable.Create<AudioBook[]>(observer =>
            {
                var callback = new SubscriptionCallback(this, observer);

                mediaBrowser.Subscribe(mediaBrowser.Root, callback);

                return Disposable.Empty;
            });
        }*/

        // ConnectionCallback class
        private sealed class ConnectionCallback : MediaBrowserCompat.ConnectionCallback
        {
            private readonly MediaBrowserServiceConnector connector;
            private readonly IObserver<Unit> connected;
            private readonly IObserver<AudioBook[]> library;
            private readonly MediaControllerCallback controllerCallback;
            private MediaControllerCompat mediaController;

            public ConnectionCallback(
                MediaBrowserServiceConnector connector,
                IObserver<Unit> connected,
                IObserver<AudioBook[]> library)
            {
                this.connector = connector;
                this.connected = connected;
                this.library = library;

                controllerCallback = new MediaControllerCallback(connector);
            }

            public override void OnConnected()
            {
                mediaController = new MediaControllerCompat(Application.Context, connector.SessionToken);
                mediaController.RegisterCallback(controllerCallback);

                connected.OnNext(Unit.Default);
                connected.OnCompleted();

                var callback = new SubscriptionCallback(connector, library);

                connector.mediaBrowser.Subscribe(connector.mediaBrowser.Root, callback);
            }

            public override void OnConnectionSuspended()
            {
                System.Diagnostics.Debug.WriteLine("[ConnectionCallback] [OnConnectionSuspended] Execute");
            }

            public override void OnConnectionFailed()
            {
                connected.OnError(new Exception());
            }

            protected override void Dispose(bool disposing)
            {
                if (disposing)
                {
                    mediaController.UnregisterCallback(controllerCallback);
                }
            }

            // MediaControllerCallback class
            private sealed class MediaControllerCallback : MediaControllerCompat.Callback
            {
                private readonly MediaBrowserServiceConnector connector;

                public MediaControllerCallback(MediaBrowserServiceConnector connector)
                {
                    this.connector = connector;
                }

                public override void OnPlaybackStateChanged(PlaybackStateCompat state)
                {
                    System.Diagnostics.Debug.WriteLine("[MediaBrowserServiceConnector.MediaControllerCallback] [OnPlaybackStateChanged] Execute");
                    base.OnPlaybackStateChanged(state);
                }

                public override void OnMetadataChanged(MediaMetadataCompat metadata)
                {
                    System.Diagnostics.Debug.WriteLine("[MediaBrowserServiceConnector.MediaControllerCallback] [OnMetadataChanged] Execute");
                }

                public override void OnQueueChanged(IList<MediaSessionCompat.QueueItem> queue)
                {
                    System.Diagnostics.Debug.WriteLine("[MediaBrowserServiceConnector.MediaControllerCallback] [OnQueueChanged] Execute");
                }

                public override void OnSessionDestroyed()
                {
                    System.Diagnostics.Debug.WriteLine("[MediaBrowserServiceConnector.MediaControllerCallback] [OnSessionDestroyed] Execute");
                }

                public override void OnSessionEvent(string e, Bundle extras)
                {
                    System.Diagnostics.Debug.WriteLine("[MediaBrowserServiceConnector.MediaControllerCallback] [OnSessionEvent] Execute");
                }

                public override void OnSessionReady()
                {
                    System.Diagnostics.Debug.WriteLine("[MediaBrowserServiceConnector.MediaControllerCallback] [OnSessionReady] Execute");
                }
            }
        }

        // ItemCallback class
        private sealed class ItemCallback : MediaBrowserCompat.ItemCallback
        {
            private readonly MediaBrowserServiceConnector connector;

            public ItemCallback(MediaBrowserServiceConnector connector)
            {
                this.connector = connector;
            }

            public override void OnError(string itemId)
            {
                System.Diagnostics.Debug.WriteLine($"[ItemCallback] [OnError] Item: \"{itemId}\"");
            }

            public override void OnItemLoaded(MediaBrowserCompat.MediaItem item)
            {
                System.Diagnostics.Debug.WriteLine($"[ItemCallback] [OnItemLoaded] Loaded: \"{item.MediaId}\"");
            }
        }

        // SubscriptionCallback class
        private sealed class SubscriptionCallback : MediaBrowserCompat.SubscriptionCallback
        {
            private readonly MediaBrowserServiceConnector connector;
            private readonly IObserver<AudioBook[]> observer;
            private readonly AudioBookBuilder builder;

            public SubscriptionCallback(MediaBrowserServiceConnector connector, IObserver<AudioBook[]> observer)
            {
                this.connector = connector;
                this.observer = observer;
                builder = new PublicAudioBookBuilder();
            }

            public override void OnChildrenLoaded(string parentId, IList<MediaBrowserCompat.MediaItem> children)
            {
                var books = new AudioBook[children.Count];

                for (var index = 0; index < children.Count; index++)
                {
                    var source = children[index];
                    books[index] = builder.BuildAudioBookFrom(source);
                }

                observer.OnNext(books);
            }

            public override void OnError(string parentId)
            {
                observer.OnError(new Exception());
            }

        }
    }
}