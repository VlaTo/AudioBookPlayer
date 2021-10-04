using AudioBookPlayer.App.Core;
using AudioBookPlayer.App.Domain.Models;
using System;
using System.Threading.Tasks;

namespace AudioBookPlayer.App.Android.Services
{
    internal partial class AudioBooksPlaybackServiceConnector
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
                var mediaId = new MediaId(key).ToString();
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
        }
    }
}