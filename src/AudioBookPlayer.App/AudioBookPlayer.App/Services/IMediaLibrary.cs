using System;
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

    public interface IMediaLibrary : IBooksProvider
    {
        event EventHandler<AudioBooksEventArgs> QueryBooksReady;

        Task<AudioBook> GetBookAsync(long bookId, CancellationToken cancellationToken = default);

        Task SaveBookAsync(AudioBook audioBook, CancellationToken cancellationToken = default);
    }
}