using System;
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

        Task QueryBooksAsync();

        Task RefreshBooksAsync();
    }
}
