using AudioBookPlayer.App.Android.Core;
using AudioBookPlayer.App.Domain.Models;
using System;
using System.Threading.Tasks;

namespace AudioBookPlayer.App.Android.Services
{
    internal partial class MediaBrowserServiceConnector
    {
        /// <summary>
        /// The Book Items task class.
        /// </summary>
        private sealed class BookItemsTask : TaskExecutorAsync<EntityId, BookItem>
        {
            private readonly BooksLibraryTask booksLibraryTask;
            private readonly IBookItemsCache cache;

            public BookItemsTask(BooksLibraryTask booksLibraryTask, IBookItemsCache cache)
                : base(new EntityTaskLock<BookItem>())
            {
                this.booksLibraryTask = booksLibraryTask;
                this.cache = cache;
            }

            protected override async Task DoExecuteAsync(EntityId key, TaskCompletionSource<BookItem> tcs, Action callback)
            {
                var mediaId = new MediaBookId(key).ToString();
                BookItem bookItem = null;

                if (false == cache.Has(mediaId))
                {
                    var bookItems = await booksLibraryTask.ExecuteAsync();

                    for (var index = 0; index < bookItems.Count; index++)
                    {
                        var item = bookItems[index];

                        if (key != item.Id)
                        {
                            continue;
                        }

                        bookItem = item;

                        break;
                    }
                }
                else
                {
                    bookItem = cache.Get(mediaId);
                }

                tcs.SetResult(bookItem);

                callback.Invoke();
            }

            /*public async Task<BookItem> GetBookItemAsync(EntityId bookId)
            {
                var tcs = new TaskCompletionSource<BookItem>();

                if (AcquireTask(bookId, tcs, out var current))
                {
                    var mediaId = new MediaBookId(bookId).ToString();
                    BookItem bookItem = null;

                    if (false == cache.Has(mediaId))
                    {
                        var bookItems = await booksLibraryTask.GetLibraryAsync();

                        for (var index = 0; index < bookItems.Count; index++)
                        {
                            var item = bookItems[index];

                            if (bookId != item.Id)
                            {
                                continue;
                            }

                            bookItem = item;

                            break;
                        }
                    }
                    else
                    {
                        bookItem = cache.Get(mediaId);
                    }

                    current.SetResult(bookItem);

                    ReleaseTask(bookId, current);
                }

                return await current.Task;
            }*/

            /*private bool AcquireTask(EntityId bookId, TaskCompletionSource<BookItem> tcs, out TaskCompletionSource<BookItem> current)
            {
                lock (gate)
                {
                    if (false == completionSources.TryGetValue(bookId, out var value))
                    {
                        completionSources.Add(bookId, tcs);
                        current = tcs;

                        return true;
                    }

                    current = value;

                    return false;
                }
            }*/

            /*private void ReleaseTask(EntityId bookId, TaskCompletionSource<BookItem> tcs)
            {
                lock (gate)
                {
                    if (completionSources.TryGetValue(bookId, out var value) && tcs == value)
                    {
                        completionSources.Remove(bookId);
                    }
                }
            }*/
        }
    }
}