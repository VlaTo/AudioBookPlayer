using System;
using System.Collections.Generic;
using AudioBookPlayer.App.Models;
using Xamarin.Forms.Internals;

namespace AudioBookPlayer.App.Services
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

        public long? BookId
        {
            get;
        }

        public AudioBook Source
        {
            get;
        }

        private LibraryChange(ChangeAction action, long? bookId = null, AudioBook source = null)
        {
            Action = action;
            BookId = bookId;
            Source = source;
        }

        public static LibraryChange RemoveBook(long bookId) =>
            new LibraryChange(ChangeAction.Remove, bookId);

        public static LibraryChange UpdateBook(long bookId, AudioBook source) =>
            new LibraryChange(ChangeAction.Update, bookId, source);

        public static LibraryChange AddBook(AudioBook source) =>
            new LibraryChange(ChangeAction.Add, source: source);
    }

    internal sealed class AudioBooksComparer
    {
        public IReadOnlyList<LibraryChange> GetChanges(IReadOnlyList<AudioBook> libraryBooks, IReadOnlyList<AudioBook> actualBooks)
        {
            var changes = new List<LibraryChange>();
            var booksToAdd = new List<AudioBook>(actualBooks);

            for (var index = 0; index < libraryBooks.Count; index++)
            {
                var originalBook = libraryBooks[index];

                if (false == originalBook.Id.HasValue)
                {
                    throw new Exception();
                }

                var actualIndex = FindBookIndex(actualBooks, originalBook);

                if (0 > actualIndex)
                {
                    changes.Add(LibraryChange.RemoveBook(originalBook.Id.Value));

                    continue;
                }

                var actualBook = actualBooks[actualIndex];

                if (IsChanged(originalBook, actualBook))
                {
                    changes.Add(LibraryChange.UpdateBook(originalBook.Id.Value, actualBook));
                }

                booksToAdd.Remove(actualBook);
            }

            foreach (var book in booksToAdd)
            {
                changes.Add(LibraryChange.AddBook(book));
            }

            return changes;
        }

        private static int FindBookIndex(IReadOnlyList<AudioBook> books, AudioBook originalBook)
        {
            bool IsSameBook(AudioBook actualBook)
            {
                if (false == String.Equals(actualBook.Title, originalBook.Title))
                {
                    return false;
                }

                for (var index = 0; index < originalBook.Authors.Count; index++)
                {
                    if (actualBook.Authors.Contains(originalBook.Authors[index]))
                    {
                        continue;
                    }

                    return false;
                }

                return true;
            }

            return books.IndexOf(IsSameBook);
        }

        private static bool IsChanged(AudioBook originalBook, AudioBook actualBook)
        {
            return false;
        }
    }
}