using AudioBookPlayer.App.Domain.Models;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AudioBookPlayer.App.Domain.Services;
using Xamarin.Forms.Internals;

namespace AudioBookPlayer.App.Services
{
    public enum ChangeAction
    {
        Add,
        Update,
        Remove
    }

    public sealed class LibraryChange
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

    /// <summary>
    /// 
    /// </summary>
    public sealed class AudioBooksLibrary
    {
        public IReadOnlyList<LibraryChange> GetChanges(IReadOnlyList<AudioBook> libraryBooks, IReadOnlyList<AudioBook> actualBooks)
        {
            var changes = new List<LibraryChange>();
            var booksToAdd = new List<AudioBook>(actualBooks);

            for (var index = 0; index < libraryBooks.Count; index++)
            {
                var originalBook = libraryBooks[index];
                var actualIndex = FindBookIndex(actualBooks, originalBook);

                if (0 > actualIndex)
                {
                    changes.Add(LibraryChange.RemoveBook(originalBook));

                    continue;
                }

                var actualBook = actualBooks[actualIndex];

                if (IsChanged(originalBook, actualBook))
                {
                    changes.Add(LibraryChange.UpdateBook(actualBook));
                }

                booksToAdd.Remove(actualBook);
            }

            foreach (var book in booksToAdd)
            {
                changes.Add(LibraryChange.AddBook(book));
            }

            return changes;
        }


        public async Task<bool> TryApplyChangesAsync(IBooksService booksService, IReadOnlyList<LibraryChange> changes, CancellationToken cancellationToken = default)
        {
            var changesApplied = 0;

                for (var changeIndex = 0; changeIndex < changes.Count; changeIndex++)
                {
                    var change = changes[changeIndex];

                    switch (change.Action)
                    {
                        case ChangeAction.Add:
                        {
                            booksService.SaveBook(change.Book);

                            changesApplied++;

                            break;
                        }

                        case ChangeAction.Remove:
                        {
                            booksService.RemoveBook(change.Book);

                            changesApplied++;

                            break;
                        }

                        default:
                        {
                            break;
                        }
                    }
                }

            return 0 < changesApplied;
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