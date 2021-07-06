using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AudioBookPlayer.App.Models;

namespace AudioBookPlayer.App.Services
{
    public sealed class AudioBooksEventArgs : EventArgs
    {
        public AudioBook[] Books
        {
            get;
        }

        public AudioBooksEventArgs(AudioBook[] books)
        {
            Books = books;
        }
    }

    public interface IBookShelfProvider
    {
        event EventHandler<AudioBooksEventArgs> QueryBooksReady;

        Task<IEnumerable<AudioBook>> QueryBooksAsync(CancellationToken cancellationToken = default);

        Task RefreshBooksAsync(CancellationToken cancellationToken = default);

        Task<AudioBook> GetBookAsync(long bookId, CancellationToken cancellationToken = default);
    }
}
