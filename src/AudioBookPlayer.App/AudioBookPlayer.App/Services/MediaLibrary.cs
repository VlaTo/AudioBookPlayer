using AudioBookPlayer.App.Core;
using AudioBookPlayer.App.Data;
using AudioBookPlayer.App.Data.Models;
using AudioBookPlayer.App.Models;
using LibraProgramming.Xamarin.Dependency.Container.Attributes;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;
using BookImage = AudioBookPlayer.App.Data.Models.BookImage;
using Chapter = AudioBookPlayer.App.Data.Models.Chapter;

namespace AudioBookPlayer.App.Services
{
    internal sealed class MediaLibrary : IMediaLibrary
    {
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
        public MediaLibrary(
            ApplicationSettings settings,
            IBookShelfDataContext context,
            IAudioBookFactoryProvider audioBookFactoryProvider)
        {
            this.settings = settings;
            this.context = context;
            this.audioBookFactoryProvider = audioBookFactoryProvider;

            queryBooksReady = new WeakEventManager();
        }

        public Task<IReadOnlyList<AudioBook>> QueryBooksAsync(CancellationToken cancellationToken = default)
        {
            /*
            //await context.DeleteAllAsync();

            var books = await context.Books
                .Include(book => book.AuthorBooks)
                .ThenInclude(ab => ab.Author)
                .Include(book => book.Chapters)
                .ThenInclude(chapter => chapter.SourceFile)
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
                FillChapters(result[index], book.Chapters);
                FillSourceFiles(result[index], book.SourceFiles);
            }

            queryBooksReady.HandleEvent(this, new AudioBooksEventArgs(result), nameof(QueryBooksReady));
            */

            return Task.FromResult<IReadOnlyList<AudioBook>>(new List<AudioBook>());
        }

        /*public async Task RefreshBooksAsync(CancellationToken cancellationToken = default)
        {
            var status = await permissionRequestor.CheckAndRequestMediaPermissionsAsync();

            if (PermissionStatus.Denied == status)
            {
                return;
            }

            await EnumerateBooksAsync(settings.LibraryRootPath, 0, cancellationToken);

            await QueryBooksAsync(cancellationToken);
        }*/

        public async Task<AudioBook> GetBookAsync(long bookId, CancellationToken cancellationToken = default)
        {
            var actual = await context.Books
                .Include(book => book.AuthorBooks)
                .ThenInclude(ab => ab.Author)
                .Include(book => book.Chapters)
                .ThenInclude(chapter => chapter.SourceFile)
                .Include(book => book.SourceFiles)
                .Include(book => book.Images)
                .Where(book => book.Id == bookId)
                .AsNoTracking()
                .FirstOrDefaultAsync(cancellationToken);

            if (null == actual)
            {
                return null;
            }

            var result = new AudioBook(actual.Title, actual.Id)
            {
                Synopsis = actual.Synopsis
            };

            FillAuthors(result, actual.AuthorBooks);
            FillImages(result, actual.Images);
            FillChapters(result, actual.Chapters);
            FillSourceFiles(result, actual.SourceFiles);

            return result;
        }

        public Task SaveBookAsync(AudioBook audioBook, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        /*private async Task EnumerateBooksAsync(string path, int level, CancellationToken cancellation = default)
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
                    : AddNewBookAsync(audioBook, cancellation);

                await task;

                await Task.CompletedTask;
            }

            // enumerate sub-folders
            var next = level + 1;

            foreach (var directory in Directory.EnumerateDirectories(path))
            {
                await EnumerateBooksAsync(directory, next);
            }
        }*/

        /*private async Task<Book> GetExistingBookAsync(AudioBook audioBook, CancellationToken cancellation = default)
        {
            var existing = await context.Books
                .Where(book => book.Title == audioBook.Title)
                .AsNoTracking()
                .SingleOrDefaultAsync(cancellation);

            return existing;
        }*/

        /*private Task UpdateExistingBookAsync(Book original, AudioBook source, CancellationToken cancellation = default)
        {
            throw new NotImplementedException();
        }*/

        /*private async Task AddNewBookAsync(AudioBook source, CancellationToken cancellation = default)
        {
            using (var transaction = await context.BeginTransactionAsync(cancellation))
            {
                var book = await AddBookAsync(source, cancellation);

                await AddBookAuthorsAsync(source, book, cancellation);
                await AddBookChaptersAsync(source, book, cancellation);
                await AddBookImagesAsync(source, book, cancellation);

                await context.SaveChangesAsync(cancellation);
                await transaction.CommitAsync(cancellation);
            }
        }*/

        /*private async Task<Book> AddBookAsync(AudioBook source, CancellationToken cancellation)
        {
            var book = new Book
            {
                Title = source.Title,
                IsExcluded = false,
                AddedToLibrary = DateTime.UtcNow,
                Synopsis = source.Synopsis,
                Duration = source.Duration,
                AuthorBooks = new List<AuthorBook>(),
                SourceFiles = new List<SourceFile>(),
                Images = new List<BookImage>(),
                Chapters = new List<Chapter>()
            };

            await context.Books.AddAsync(book, cancellation);

            return book;
        }*/

        /*private async Task AddBookAuthorsAsync(AudioBook source, Book book, CancellationToken cancellation)
        {
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
        }*/

        /*private async Task AddBookChaptersAsync(AudioBook source, Book book, CancellationToken cancellation)
        {
            foreach (var audioBookChapter in source.Chapters)
            {
                var filename = audioBookChapter.SourceFile;
                var sourceFile = await context.SourceFiles
                    .Where(s => s.Filename == filename)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(cancellation);

                if (null == sourceFile)
                {
                    var file = new FileInfo(filename);
                    
                    sourceFile = new SourceFile
                    {
                        Filename = filename,
                        Created = file.CreationTimeUtc,
                        Modified = file.LastAccessTimeUtc,
                        Length = file.Length,
                        Book = book
                    };

                    await context.SourceFiles.AddAsync(sourceFile, cancellation);

                    book.SourceFiles.Add(sourceFile);
                }

                var chapter = new Chapter
                {
                    Title = audioBookChapter.Title,
                    Offset = audioBookChapter.Start,
                    Length = audioBookChapter.Duration,
                    Book = book,
                    SourceFile = sourceFile
                };

                await context.Chapters.AddAsync(chapter, cancellation);

                book.Chapters.Add(chapter);
            }
        }*/

        /*private async Task AddBookImagesAsync(AudioBook source, Book book, CancellationToken cancellation)
        {
            foreach (var audioBookImage in source.Images)
            {
                var bookImage = new BookImage
                {
                    Blob = await audioBookImage.GetBytesAsync(cancellation),
                    Name = audioBookImage.Key,
                    Book = book
                };

                await context.BookImages.AddAsync(bookImage, cancellation);

                book.Images.Add(bookImage);
            }
        }*/

        private static void FillAuthors(AudioBook audioBook, ICollection<AuthorBook> bookAuthors)
        {
            foreach (var bookAuthor in bookAuthors)
            {
                //audioBook.Authors.Add(bookAuthor.Author.Name);
            }
        }

        private static void FillImages(AudioBook audioBook, ICollection<BookImage> bookImages)
        {
            foreach (var image in bookImages)
            {
                //var item = new InMemoryAudioBookImage(image.Name, image.Blob);
                //audioBook.Images.Add(item);
            }
        }

        private void FillChapters(AudioBook audioBook, ICollection<Chapter> chapters)
        {
            foreach (var chapter in chapters)
            {
                /*var audioBookChapter = new AudioBookChapter
                {
                    Title = chapter.Title,
                    Start = chapter.Offset,
                    Duration = chapter.Length,
                    SourceFile = chapter.SourceFile.Filename
                };

                audioBook.Chapters.Add(audioBookChapter);*/
            }
        }

        private void FillSourceFiles(AudioBook audioBook, ICollection<SourceFile> sourceFiles)
        {
            foreach (var sourceFile in sourceFiles)
            {
                ;
            }
        }
    }
}
