using AudioBookPlayer.App.Core;
using AudioBookPlayer.App.Data;
using AudioBookPlayer.App.Data.Models;
using AudioBookPlayer.App.Models;
using LibraProgramming.Xamarin.Dependency.Container.Attributes;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BookImage = AudioBookPlayer.App.Data.Models.BookImage;
using Chapter = AudioBookPlayer.App.Data.Models.Chapter;

namespace AudioBookPlayer.App.Services
{
    internal sealed class MediaLibrary : IMediaLibrary
    {
        private readonly IMediaLibraryDataContext context;
        private readonly IAudioBookFactoryProvider audioBookFactoryProvider;
        private readonly ApplicationSettings settings;

        [PrefferedConstructor]
        public MediaLibrary(
            ApplicationSettings settings,
            IMediaLibraryDataContext context,
            IAudioBookFactoryProvider audioBookFactoryProvider)
        {
            this.settings = settings;
            this.context = context;
            this.audioBookFactoryProvider = audioBookFactoryProvider;
        }

        public async Task<IReadOnlyList<AudioBook>> QueryBooksAsync(CancellationToken cancellationToken = default)
        {
            //await context.DeleteAllAsync(cancellationToken);

            var books = await context.Books
                .Include(book => book.AuthorBooks)
                .ThenInclude(ab => ab.Author)
                .Include(book => book.Chapters)
                .ThenInclude(chapter => chapter.SourceFile)
                .Include(book => book.SourceFiles)
                .Include(book => book.Images)
                .Where(book => !book.DoNotShow)
                .OrderBy(book => book.Id)
                .AsNoTracking()
                .ToArrayAsync(cancellationToken);

            var audioBooks = new AudioBook[books.Length];

            for (var index = 0; index < books.Length; index++)
            {
                var source = books[index];
                var audioBook = new AudioBook(source.Title, source.Id)
                {
                    Synopsis = source.Synopsis
                };

                audioBooks[index] = audioBook;

                FillAuthors(audioBook, source.AuthorBooks);
                FillImages(audioBook, source.Images);
                FillChapters(audioBook, source.Chapters);
                //FillSourceFiles(audioBook, source.SourceFiles);
            }

            return audioBooks;
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
            //FillSourceFiles(result, actual.SourceFiles);

            return result;
        }

        public async Task SaveBookAsync(AudioBook audioBook, CancellationToken cancellationToken = default)
        {
            if (audioBook.Id.HasValue)
            {

            }
            else
            {
                await AddNewBookAsync(audioBook, cancellationToken);
            }
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

        private async Task AddNewBookAsync(AudioBook source, CancellationToken cancellation = default)
        {
            using (var transaction = await context.BeginTransactionAsync(cancellation))
            {
                var book = await AddBookAsync(source, cancellation);

                await AddBookAuthorsAsync(book, source, cancellation);
                await AddBookChaptersAsync(book, source, cancellation);
                //await AddBookImagesAsync(source, book, cancellation);

                await context.SaveChangesAsync(cancellation);
                await transaction.CommitAsync(cancellation);

                source.Id = book.Id;
            }
        }

        private async Task<Book> AddBookAsync(AudioBook source, CancellationToken cancellation)
        {
            var book = new Book
            {
                Title = source.Title,
                DoNotShow = false,
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
        }

        private async Task AddBookAuthorsAsync(Book book, AudioBook source, CancellationToken cancellation)
        {
            for (var index = 0; index < source.Authors.Count; index++)
            {
                var author = source.Authors[index];
                var entity = await context.Authors
                    .Where(a => a.Name == author.Name)
                    //.AsNoTracking()
                    .FirstOrDefaultAsync(cancellation);

                if (null == entity)
                {
                    entity = new Author
                    {
                        Name = author.Name,
                        AuthorBooks = new List<AuthorBook>()
                    };

                    await context.Authors.AddAsync(entity, cancellation);
                }

                book.AuthorBooks.Add(new AuthorBook
                {
                    Book = book,
                    Author = entity
                });
            }
        }

        private async Task AddBookChaptersAsync(Book book, AudioBook source, CancellationToken cancellation)
        {
            for (var chapterIndex = 0; chapterIndex < source.Chapters.Count; chapterIndex++)
            {
                var sourceChapter = source.Chapters[chapterIndex];

                for (var index = 0; index < sourceChapter.Fragments.Count; index++)
                {
                    var sourceFragment = sourceChapter.Fragments[index];
                    var sourceFile = new SourceFile
                    {
                        Filename = sourceFragment.SourceFile.ContentUri,
                        //Created = file.CreationTimeUtc,
                        //Modified = file.LastAccessTimeUtc,
                        //Length = file.Length,
                        Book = book
                    };

                    var chapter = new Chapter
                    {
                        Position = chapterIndex,
                        Title = sourceChapter.Title,
                        Offset = sourceChapter.Start,
                        Length = sourceChapter.Duration,
                        Book = book,
                        SourceFile = sourceFile
                    };

                    // Debug.WriteLine($"[MediaLibrary] [AddBookChaptersAsync] ({sourceChapter.Start:g}, {sourceChapter.Duration:g}) '{sourceChapter.Title}'");

                    book.SourceFiles.Add(sourceFile);
                    book.Chapters.Add(chapter);

                    await context.SourceFiles.AddAsync(sourceFile, cancellation);
                    await context.Chapters.AddAsync(chapter, cancellation);
                }
            }
        }

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
                var author = new AudioBookAuthor(bookAuthor.Author.Name);
                audioBook.Authors.Add(author);
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

        private void FillChapters(AudioBook audioBook, ICollection<Chapter> bookChapters)
        {
            foreach (var bookChapter in bookChapters.OrderBy(chapter => chapter.Position))
            {
                var chapter = new AudioBookChapter(audioBook, bookChapter.Title, bookChapter.Offset);
                var sourceFile = GetOrCreateSourceFile(audioBook, bookChapter.SourceFile);
                var fragment = new AudioBookChapterFragment(bookChapter.Offset, bookChapter.Length, sourceFile);

                audioBook.Chapters.Add(chapter);
                chapter.Fragments.Add(fragment);
            }
        }

        private static AudioBookSourceFile GetOrCreateSourceFile(AudioBook audioBook, SourceFile sourceFile)
        {
            var contentUri = sourceFile.Filename;

            foreach (var bookSourceFile in audioBook.SourceFiles)
            {
                if (String.Equals(contentUri, bookSourceFile.ContentUri))
                {
                    return bookSourceFile;
                }
            }

            var source = new AudioBookSourceFile(audioBook, contentUri, sourceFile.Length);

            audioBook.SourceFiles.Add(source);

            return source;
        }

        /*private void FillSourceFiles(AudioBook audioBook, ICollection<SourceFile> sourceFiles)
        {
            foreach (var sourceFile in sourceFiles)
            {
                ;
            }
        }*/
    }
}
