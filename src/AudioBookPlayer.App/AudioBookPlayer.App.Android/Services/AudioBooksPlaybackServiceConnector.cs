﻿using Android.Content;
using Android.OS;
using Android.Support.V4.Media;
using Android.Support.V4.Media.Session;
using AudioBookPlayer.App.Android.Extensions;
using AudioBookPlayer.App.Core;
using AudioBookPlayer.App.Domain.Models;
using AudioBookPlayer.App.Services;
using LibraProgramming.Xamarin.Core;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;
using Application = Android.App.Application;
using ResultReceiver = Android.OS.ResultReceiver;

namespace AudioBookPlayer.App.Android.Services
{
    internal sealed partial class AudioBooksPlaybackServiceConnector : Java.Lang.Object, IMediaBrowserServiceConnector
    {
        private readonly IBookItemsCache bookItemsCache;
        private readonly MediaControllerCallback mediaControllerCallback;
        private readonly MediaBrowserCompat mediaBrowser;
        private readonly ServiceConnectTask serviceConnectTask;
        private readonly BooksLibraryTask booksLibraryTask;
        private readonly BookItemsTask bookItemsTask;
        private readonly BookSectionsTask bookSectionsTask;
        private readonly WeakEventManager eventManager;
        private MediaControllerCompat mediaController;

        public bool IsConnected => mediaBrowser is { IsConnected: true };

        public MediaSessionCompat.Token SessionToken => mediaBrowser.SessionToken;

        public MediaControllerCompat MediaController
        {
            get => mediaController;
            private set
            {
                if (ReferenceEquals(mediaController, value))
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

        public event EventHandler<PlaybackStateEventArgs> PlaybackStateChanged
        {
            add => eventManager.AddEventHandler(value);
            remove => eventManager.RemoveEventHandler(value);
        }

        public AudioBooksPlaybackServiceConnector(IBookItemsCache bookItemsCache)
        {
            this.bookItemsCache = bookItemsCache;

            var serviceName = Java.Lang.Class.FromType(typeof(AudioBooksPlaybackService)).Name;
            var componentName = new ComponentName(Application.Context, serviceName);
            TaskCompletionSource connect = null;
            var connectionCallback = new ServiceConnectionCallback
            {
                OnConnectImpl = () =>
                {
                    MediaController = new MediaControllerCompat(Application.Context, SessionToken);
                    connect?.Complete();
                },
                OnConnectionSuspendedImpl = () => connect?.Cancel(),
                OnConnectionFailedImpl = () => connect?.Fail(new Exception())
            };

            mediaBrowser = new MediaBrowserCompat(Application.Context, componentName, connectionCallback, null);
            mediaControllerCallback = new MediaControllerCallback
            {
                OnSessionReadyImpl = () =>
                {
                    System.Diagnostics.Debug.WriteLine("[MediaBrowserServiceConnector.MediaControllerCallback] [OnSessionReady] Executed");
                },
                OnPlaybackStateChangedImpl = OnPlaybackStateChanged,
                OnMetadataChangedImpl = (metadata) =>
                {
                    System.Diagnostics.Debug.WriteLine("[MediaBrowserServiceConnector.MediaControllerCallback] [OnMetadataChanged] Execute");
                },
                OnQueueTitleChangedImpl = (title) =>
                {
                    System.Diagnostics.Debug.WriteLine("[MediaBrowserServiceConnector.MediaControllerCallback] [OnQueueTitleChanged] Execute");
                },
                OnQueueChangedImpl = (queue) =>
                {
                    System.Diagnostics.Debug.WriteLine("[MediaBrowserServiceConnector.MediaControllerCallback] [OnQueueChanged] Execute");
                },
                OnAudioInfoChangedImpl = (playbackInfo) =>
                {
                    System.Diagnostics.Debug.WriteLine("[MediaBrowserServiceConnector.MediaControllerCallback] [OnAudioInfoChanged] Execute");
                },
                OnSessionEventImpl = (eventName, options) =>
                {
                    System.Diagnostics.Debug.WriteLine($"[MediaBrowserServiceConnector.MediaControllerCallback] [OnSessionEvent] Event: '{eventName}' received");
                }
            };

            serviceConnectTask = new ServiceConnectTask
            {
                OnExecuteImpl = (tcs, cb) =>
                {
                    connect = tcs;
                    mediaBrowser.Connect();
                }
            };

            eventManager = new WeakEventManager();
            booksLibraryTask = new BooksLibraryTask(mediaBrowser, bookItemsCache);
            bookItemsTask = new BookItemsTask(booksLibraryTask, bookItemsCache);
            bookSectionsTask = new BookSectionsTask(mediaBrowser);
        }

        /// <inheritdoc cref="IMediaBrowserServiceConnector.ConnectAsync" />
        public Task ConnectAsync() => serviceConnectTask.ExecuteAsync();

        /// <inheritdoc cref="IMediaBrowserServiceConnector.GetLibraryAsync" />
        public Task<IReadOnlyList<BookItem>> GetLibraryAsync() => booksLibraryTask.ExecuteAsync();

        /// <inheritdoc cref="IMediaBrowserServiceConnector.GetBookItemAsync" />
        public Task<BookItem> GetBookItemAsync(EntityId bookId) => bookItemsTask.ExecuteAsync(bookId);

        /// <inheritdoc cref="IMediaBrowserServiceConnector.GetSectionsAsync"/>
        public Task<IReadOnlyList<SectionItem>> GetSectionsAsync(EntityId bookId) => bookSectionsTask.ExecuteAsync(bookId);

        /*{
            var tcs = new TaskCompletionSource<IReadOnlyList<SectionItem>>();

            if (AcquireSectionTask(bookId, tcs, out var current))
            {
                var mediaId = new MediaBookId(bookId).ToString();
                var callback = new SubscriptionCallback();

                void Unsubscribe()
                {
                    mediaBrowser.Unsubscribe(mediaId, callback);
                    ReleaseSectionTask(bookId, current);
                }

                callback.ChildrenLoadedImpl = (parentId, children) =>
                {
                    var sectionItems = new SectionItem[children.Count];

                    for (var index = 0; index < children.Count; index++)
                    {
                        var mediaItem = children[index];
                        sectionItems[index] = mediaItem.ToSectionItem();
                    }

                    current.SetResult(sectionItems);

                    Unsubscribe();
                };

                callback.ErrorImpl = parentId =>
                {
                    library.TrySetException(new Exception());
                    Unsubscribe();
                };

                var options = new Bundle();

                mediaBrowser.Subscribe(mediaId, options, callback);
            }

            return current.Task;
        }*/

        /// <inheritdoc cref="IMediaBrowserServiceConnector.UpdateLibraryAsync" />
        public Task UpdateLibraryAsync()
        {
            var tcs = new TaskCompletionSource();

            {
                var options = Bundle.Empty;
                var handler = new Handler(Looper.MainLooper, new ActionHandlerCallback(message =>
                {
                    System.Diagnostics.Debug.WriteLine("[ConnectionCallback.OnConnected] [Handler] Callback executed");
                    tcs.Complete();
                }));

                MediaController.SendCommand(
                    AudioBooksPlaybackService.IAudioBookMediaBrowserService.UpdateLibrary,
                    options,
                    new ResultReceiver(handler)
                );
            }

            return tcs.Task;
        }

        public void Play(EntityId bookId)
        {
            var controls = MediaController?.GetTransportControls();

            if (null != controls)
            {
                var mediaId = new MediaId(bookId).ToString();
                var options = new Bundle();
                controls.PlayFromMediaId(mediaId, options);
            }
        }

        public void Pause()
        {
            var controls = MediaController?.GetTransportControls();

            if (null != controls)
            {
                controls.Pause();
            }
        }

        private void OnPlaybackStateChanged(PlaybackStateCompat playback)
        {
            var state = playback.State.ToPlaybackState();
            var eventArgs = new PlaybackStateEventArgs(state);

            eventManager.HandleEvent(this, eventArgs, nameof(PlaybackStateChanged));
        }

        // ConnectionCallback class
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

        private enum ConnectStatus
        {
            Failed = -1,
            Success,
            Cancelled
        }

        // ItemCallback class
        private sealed class ItemCallback : MediaBrowserCompat.ItemCallback
        {
            private readonly AudioBooksPlaybackServiceConnector connector;

            public ItemCallback(AudioBooksPlaybackServiceConnector connector)
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

        // ActionHandlerCallback class
        private sealed class ActionHandlerCallback : Java.Lang.Object, Handler.ICallback
        {
            private readonly Action<Message> handler;

            public ActionHandlerCallback(Action<Message> handler)
            {
                this.handler = handler;
            }

            public bool HandleMessage(Message msg)
            {
                handler.Invoke(msg);
                return true;
            }
        }

        // MediaControllerCallback class
        /*private sealed class MediaControllerCallback : MediaControllerCompat.Callback
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
        }*/
    }
}