using Android.OS;
using Android.Support.V4.Media;
using AudioBookPlayer.App.Android.Extensions;
using AudioBookPlayer.App.Domain.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AudioBookPlayer.App.Android.Services
{
    internal partial class MediaBrowserServiceConnector
    {
        /// <summary>
        /// The Book Items task class.
        /// </summary>
        private sealed class BooksLibraryTask : TaskExecutor<IReadOnlyList<BookItem>>
        {
            private readonly MediaBrowserCompat mediaBrowser;
            private readonly IBookItemsCache cache;

            public BooksLibraryTask(MediaBrowserCompat mediaBrowser, IBookItemsCache cache)
                : base(new InterlockedTaskLock<IReadOnlyList<BookItem>>())
            {
                this.mediaBrowser = mediaBrowser;
                this.cache = cache;
            }

            protected override void DoExecute(TaskCompletionSource<IReadOnlyList<BookItem>> tcs, Action callback)
            {
                var mediaId = mediaBrowser.Root;
                var subscriptionCallback = new SubscriptionCallback();

                subscriptionCallback.ChildrenLoadedImpl = (parentId, children) =>
                {
                    var bookItems = new BookItem[children.Count];

                    for (var index = 0; index < children.Count; index++)
                    {
                        var mediaItem = children[index];

                        bookItems[index] = mediaItem.ToBookItem();
                        cache.Put(mediaItem.MediaId, bookItems[index]);
                    }

                    tcs.SetResult(bookItems);
                    mediaBrowser.Unsubscribe(mediaId, subscriptionCallback);

                    callback.Invoke();
                };

                subscriptionCallback.ErrorImpl = parentId =>
                {
                    tcs.TrySetException(new Exception());
                    mediaBrowser.Unsubscribe(mediaId, subscriptionCallback);
                    
                    callback.Invoke();
                };

                var options = new Bundle();

                mediaBrowser.Subscribe(mediaId, options, subscriptionCallback);
            }

            /*public Task<IReadOnlyList<BookItem>> GetLibraryAsync()
            {
                var tcs = new TaskCompletionSource<IReadOnlyList<BookItem>>();

                if (null == Interlocked.CompareExchange(ref completionSource, tcs, null))
                {
                    var mediaId = mediaBrowser.Root;
                    var callback = new SubscriptionCallback();

                    void Unsubscribe()
                    {
                        mediaBrowser.Unsubscribe(mediaId, callback);
                        Interlocked.CompareExchange(ref completionSource, null, tcs);
                    }

                    callback.ChildrenLoadedImpl = (parentId, children) =>
                    {
                        var bookItems = new BookItem[children.Count];

                        for (var index = 0; index < children.Count; index++)
                        {
                            var mediaItem = children[index];
                            bookItems[index] = mediaItem.ToBookItem();
                            cache.Put(mediaItem.MediaId, bookItems[index]);
                        }

                        completionSource.SetResult(bookItems);

                        Unsubscribe();
                    };

                    callback.ErrorImpl = parentId =>
                    {
                        completionSource.TrySetException(new Exception());
                        Unsubscribe();
                    };

                    var options = new Bundle();

                    mediaBrowser.Subscribe(mediaId, options, callback);
                }

                return completionSource.Task;
            }*/
        }
    }
}