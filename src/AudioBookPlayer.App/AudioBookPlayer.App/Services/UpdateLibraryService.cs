using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AudioBookPlayer.App.Domain.Models;
using AudioBookPlayer.App.Domain.Providers;
using AudioBookPlayer.App.Domain.Services;
using Xamarin.Forms.Internals;

namespace AudioBookPlayer.App.Services
{
    internal sealed class UpdateLibraryService : IUpdateLibraryService
    {
        private readonly IBooksProvider booksProvider;
        private readonly IBooksService booksService;
        private readonly ICoverService coverService;

        public UpdateLibraryService(
            IBooksProvider booksProvider,
            IBooksService booksService,
            ICoverService coverService)
        {
            this.booksProvider = booksProvider;
            this.booksService = booksService;
            this.coverService = coverService;
        }

        public void Update(IProgress<int> progress)
        {
            progress.Report(0);

            // 1. Get books from device and library
            var actualBooks = booksProvider.QueryBooks();
            var libraryBooks = booksService.QueryBooks();

            // 2. Compare collections, get differences
            var changes = GetChanges(libraryBooks, actualBooks);
            // 3. Apply differences to library
            if (0 < changes.Count)
            {
                ApplyChanges(booksService, changes, progress);
            }
        }

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
        
        public void ApplyChanges(IBooksService booksService, IReadOnlyList<LibraryChange> changes, IProgress<int> progress)
        {
            for (var changeIndex = 0; changeIndex < changes.Count; changeIndex++)
            {
                var change = changes[changeIndex];

                switch (change.Action)
                {
                    case ChangeAction.Add:
                    {
                        booksService.SaveBook(change.Book);

                        break;
                    }

                    case ChangeAction.Remove:
                    {
                        booksService.RemoveBook(change.Book);

                        break;
                    }

                    default:
                    {
                        break;
                    }
                }
            }
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
            if (originalBook.Authors.Count != actualBook.Authors.Count)
            {
                return true;
            }

            for (var authorIndex = 0; authorIndex < originalBook.Authors.Count; authorIndex++)
            {
                var originalBookAuthor = originalBook.Authors[authorIndex];

                if (String.Equals(originalBookAuthor, actualBook.Authors[authorIndex]))
                {
                    continue;
                }

                return true;
            }

            if (false == String.Equals(originalBook.Synopsis, actualBook.Synopsis))
            {
                return true;
            }

            return false;
        }
    }
}