using AudioBookPlayer.App.Core;
using AudioBookPlayer.App.Data;
using AudioBookPlayer.App.Data.Models;
using AudioBookPlayer.App.Models;
using LibraProgramming.Xamarin.Dependency.Container.Attributes;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Essentials;
using BookImage = AudioBookPlayer.App.Data.Models.BookImage;

namespace AudioBookPlayer.App.Services
{
    internal sealed class BookShelfProvider : IBookShelfProvider
    {
        private readonly IPermissionRequestor permissionRequestor;
        private readonly IBookShelfDataContext context;
        private readonly IAudioBookFactoryProvider audioBookFactoryProvider;
        private readonly ApplicationSettings settings;
        private readonly WeakEventManager queryBooksReady;

        public event EventHandler<AudioBooksEventArgs> QueryBooksReady
        {
            add => queryBooksReady.AddEventHandler(value);
            remove => queryBooksReady.RemoveEventHandler(value);
        }

        [PrefferedConstructor]
        public BookShelfProvider(
            ApplicationSettings settings,
            IBookShelfDataContext context,
            IAudioBookFactoryProvider audioBookFactoryProvider,
            IPermissionRequestor permissionRequestor)
        {
            this.settings = settings;
            this.context = context;
            this.audioBookFactoryProvider = audioBookFactoryProvider;
            this.permissionRequestor = permissionRequestor;

            queryBooksReady = new WeakEventManager();
        }

        public async Task QueryBooksAsync(CancellationToken cancellationToken = default)
        {

            //await context.DeleteAllAsync();

            var books = await context.Books
                .Include(book => book.AuthorBooks)
                .ThenInclude(ab => ab.Author)
                .Include(book => book.SourceFiles)
                .Include(book => book.Images)
                .Where(book => !book.IsExcluded)
                .AsNoTracking()
                .ToArrayAsync(cancellationToken);

            var result = new AudioBook[books.Length];

            for (var index = 0; index < books.Length; index++)
            {
                var book = books[index];

                result[index] = new AudioBook
                {
                    Id = book.Id,
                    Title = book.Title,
                    Synopsis = book.Synopsis,
                    Duration = book.Duration
                };

                FillAuthors(result[index], book.AuthorBooks);
                FillImages(result[index], book.Images);
            }

            queryBooksReady.RaiseEvent(this, new AudioBooksEventArgs(result), nameof(QueryBooksReady));
        }

        public async Task RefreshBooksAsync(CancellationToken cancellationToken = default)
        {
            var status = await permissionRequestor.CheckAndRequestMediaPermissionsAsync();

            if (PermissionStatus.Denied == status)
            {
                return;
            }

            await EnumerateBooksAsync(settings.LibraryRootPath, 0, cancellationToken);

            await QueryBooksAsync(cancellationToken);
        }

        public async Task<AudioBook> GetBookAsync(long bookId, CancellationToken cancellationToken = default)
        {
            var actual = await context.Books
                .Include(book => book.AuthorBooks)
                .ThenInclude(ab => ab.Author)
                .Include(book => book.SourceFiles)
                .Include(book => book.Images)
                .Where(book => book.Id == bookId)
                .AsNoTracking()
                .FirstOrDefaultAsync(cancellationToken);

            if (null == actual)
            {
                return null;
            }

            var result = new AudioBook
            {
                Id = actual.Id,
                Title = actual.Title,
                Synopsis = actual.Synopsis,
                Duration = actual.Duration
            };

            FillAuthors(result, actual.AuthorBooks);
            FillImages(result, actual.Images);

            return result;
        }

        private async Task EnumerateBooksAsync(string path, int level, CancellationToken cancellation = default)
        {
            if (!Directory.Exists(path))
            {
                return;
            }

            // enumerate files
            foreach (var file in Directory.EnumerateFiles(path))
            {
                var folder = Path.GetDirectoryName(file);
                var filename = Path.GetFileName(file);
                var factory = audioBookFactoryProvider.CreateFactoryFor(Path.GetExtension(filename));

                if (null == factory)
                {
                    // not supported
                    continue;
                }

                var files = new[] {file};

                var audioBook = factory.CreateAudioBook(folder, filename, level);
                var existing = await GetExistingBookAsync(audioBook, cancellation);
                var task = (null != existing)
                    ? UpdateExistingBookAsync(existing, audioBook, cancellation)
                    : AddNewBookAsync(audioBook, files, cancellation);

                await task;
            }

            // enumerate sub-folders
            var next = level + 1;

            foreach (var directory in Directory.EnumerateDirectories(path))
            {
                await EnumerateBooksAsync(directory, next);
            }
        }

        private async Task<Book> GetExistingBookAsync(AudioBook audioBook, CancellationToken cancellation = default)
        {
            var existing = await context.Books
                .Where(book => book.Title == audioBook.Title)
                .AsNoTracking()
                .SingleOrDefaultAsync(cancellation);

            return existing;
        }

        private async Task UpdateExistingBookAsync(Book original, AudioBook source, CancellationToken cancellation = default)
        {

        }

        private async Task AddNewBookAsync(AudioBook source, string[] files, CancellationToken cancellation = default)
        {
            using (var transaction = await context.BeginTransactionAsync(cancellation))
            {
                var book = new Book
                {
                    Title = source.Title,
                    IsExcluded = false,
                    AddedToLibrary = DateTime.UtcNow,
                    Synopsis = source.Synopsis,
                    Duration = source.Duration,
                    AuthorBooks = new List<AuthorBook>()
                    //SourceFiles = new List<SourceFile>()
                };

                await context.Books.AddAsync(book, cancellation);

                foreach (var author in source.Authors)
                {
                    var actual = await context.Authors
                        .Where(a => a.Name == author)
                        .FirstOrDefaultAsync(cancellation);

                    if (null == actual)
                    {
                        actual = new Author
                        {
                            Name = author,
                            AuthorBooks = new List<AuthorBook>()
                        };

                        await context.Authors.AddAsync(actual, cancellation);
                    }

                    book.AuthorBooks.Add(new AuthorBook
                    {
                        Book = book,
                        Author = actual
                    });
                }

                foreach (var file in files)
                {
                    var sourceFile = new SourceFile
                    {
                        Source = file,
                        Created = File.GetCreationTimeUtc(file),
                        Modified = File.GetLastAccessTimeUtc(file),
                        Length = -1,
                        Book = book
                    };

                    await context.SourceFiles.AddAsync(sourceFile, cancellation);

                    book.SourceFiles.Add(sourceFile);
                }

                foreach (var image in source.Images)
                {
                    var bookImage = new BookImage
                    {
                        Blob = await image.GetBytesAsync(cancellation),
                        Name = image.Key,
                        Book = book
                    };

                    await context.BookImages.AddAsync(bookImage, cancellation);

                    book.Images.Add(bookImage);
                }

                await context.SaveChangesAsync(cancellation);

                await transaction.CommitAsync(cancellation);
            }
        }

        private static void FillAuthors(AudioBook audioBook, ICollection<AuthorBook> bookAuthors)
        {
            foreach (var bookAuthor in bookAuthors)
            {
                audioBook.Authors.Add(bookAuthor.Author.Name);
            }
        }

        private static void FillImages(AudioBook audioBook, ICollection<BookImage> bookImages)
        {
            foreach (var image in bookImages)
            {
                var item = new MemoryImage(image.Name, image.Blob);
                audioBook.Images.Add(item);
            }
        }
    }
}
