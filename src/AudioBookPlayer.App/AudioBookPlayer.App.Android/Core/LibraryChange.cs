using AudioBookPlayer.App.Domain.Models;

namespace AudioBookPlayer.App.Android.Core
{
    internal enum ChangeAction
    {
        Add,
        Update,
        Remove
    }

    internal sealed class LibraryChange
    {
        public ChangeAction Action
        {
            get;
        }

        public AudioBook Book
        {
            get;
        }

        private LibraryChange(ChangeAction action, AudioBook book)
        {
            Action = action;
            Book = book;
        }

        public static LibraryChange RemoveBook(AudioBook book) => new LibraryChange(ChangeAction.Remove, book);

        public static LibraryChange UpdateBook(AudioBook book) => new LibraryChange(ChangeAction.Update, book);

        public static LibraryChange AddBook(AudioBook book) => new LibraryChange(ChangeAction.Add, book);
    }
}