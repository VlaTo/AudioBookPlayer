using System;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V4.Media;
using Android.Support.V4.Media.Session;
using AudioBookPlayer.App.Android.Services;
using AudioBookPlayer.App.Domain.Models;
using AudioBookPlayer.App.Services;
using Java.Util;
using Xamarin.Forms;
using Application = Android.App.Application;
using Observable = System.Reactive.Linq.Observable;

[assembly: Dependency(typeof(MediaBrowserServiceConnector))]

namespace AudioBookPlayer.App.Android.Services
{
    internal sealed class MediaBrowserServiceConnector : Java.Lang.Object, IMediaBrowserServiceConnector
    {
        private readonly MediaBrowserCompat mediaBrowser;
        private readonly ConnectionCallback connectionCallback;
        //private readonly SubscriptionCallback subscriptionCallback;

        public bool IsConnected => mediaBrowser.IsConnected;

        public MediaSessionCompat.Token SessionToken => mediaBrowser.SessionToken;

        public IObservable<Unit> Connected
        {
            get;
        }

        public MediaBrowserServiceConnector()
        {
            var serviceName = Java.Lang.Class.FromType(typeof(AudioBookPlaybackService)).Name;
            var componentName = new ComponentName(Application.Context, serviceName);
            var subject = new Subject<Unit>();

            var connectable = subject.Take(1).Publish();
            
            connectable.Connect();

            Connected = connectable.AsObservable();

            connectionCallback = new ConnectionCallback(this, subject);
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

        public IObservable<AudioBook> GetRoot()
        {
            return Observable.Create<AudioBook>(observer =>
            {
                if (mediaBrowser.IsConnected)
                {
                    var id = mediaBrowser.Root;
                    var callback = new SubscriptionCallback(this, observer);

                    mediaBrowser.Subscribe(id, callback);
                }
                else
                {
                    observer.OnCompleted();
                }

                return Disposable.Empty;
            });
        }

        // ConnectionCallback class
        private sealed class ConnectionCallback : MediaBrowserCompat.ConnectionCallback
        {
            private readonly MediaBrowserServiceConnector connector;
            private readonly IObserver<Unit> observer;
            private readonly MediaControllerCallback controllerCallback;

            public ConnectionCallback(MediaBrowserServiceConnector connector, IObserver<Unit> observer)
            {
                this.connector = connector;
                this.observer = observer;

                controllerCallback = new MediaControllerCallback(connector);
            }

            public override void OnConnected()
            {
                //connector.GetRoot();

                var mediaController = new MediaControllerCompat(Application.Context, connector.SessionToken);
                
                mediaController.RegisterCallback(controllerCallback);

                observer.OnNext(Unit.Default);
                observer.OnCompleted();
            }

            public override void OnConnectionSuspended()
            {
                System.Diagnostics.Debug.WriteLine("[ConnectionCallback] [OnConnectionSuspended] Execute");
            }

            public override void OnConnectionFailed()
            {
                System.Diagnostics.Debug.WriteLine("[ConnectionCallback] [OnConnectionFailed] Execute");
            }

            protected override void Dispose(bool disposing)
            {
                System.Diagnostics.Debug.WriteLine("[ConnectionCallback] [Dispose] Execute");
                base.Dispose(disposing);
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
            private readonly IObserver<AudioBook> observer;

            public SubscriptionCallback(MediaBrowserServiceConnector connector, IObserver<AudioBook> observer)
            {
                this.connector = connector;
                this.observer = observer;
            }

            public override void OnChildrenLoaded(string parentId, IList<MediaBrowserCompat.MediaItem> children)
            {
                for (var index = 0; index < children.Count; index++)
                {
                    var source = children[index];
                    var id = long.TryParse(source.MediaId, out var value) ? new long?(value) : null;
                    var audioBook = new AudioBook(source.Description.Title, id)
                    {
                        Synopsis = source.Description.Description
                    };

                    observer.OnNext(audioBook);
                }
            }

            public override void OnError(string parentId)
            {
                System.Diagnostics.Debug.WriteLine("[SubscriptionCallback] [OnError] Execute");
            }
        }
    }
}