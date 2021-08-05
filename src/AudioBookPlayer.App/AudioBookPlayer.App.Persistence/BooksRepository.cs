using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using AudioBookPlayer.App.Domain.Data;
using AudioBookPlayer.App.Domain.Models;
using AudioBookPlayer.App.Domain.Services;
using AudioBookPlayer.App.Persistence.Models;
using Microsoft.EntityFrameworkCore;

namespace AudioBookPlayer.App.Persistence
{
    public sealed class BooksRepository : IBooksRepository
    {
        private readonly ApplicationDbContext context;
        private readonly ICoverService coverService;

        public BooksRepository(ApplicationDbContext context, ICoverService coverService)
        {
            this.context = context;
            this.coverService = coverService;
        }

        public async Task AddAsync(AudioBook model)
        {
            var book = new Book
            {
                Title = model.Title,
                DoNotShow = false,
                Synopsis = model.Synopsis,
                Duration = model.Duration,
                AuthorBooks = new List<AuthorBook>(),
                SourceFiles = new List<SourceFile>(),
                Images = new List<BookImage>(),
                Chapters = new List<Chapter>()
            };

            await context.Books.AddAsync(book);

            await AddAuthorsAsync(book, model.Authors, CancellationToken.None);
            await AddChaptersAsync(book, model.Chapters, CancellationToken.None);
            await AddCoverImagesAsync(book, model.Images, CancellationToken.None);

            model.Id = book.Id;
        }

        public async Task<AudioBook> GetAsync(long id)
        {
            var book = await context.Books
                .Include(book => book.AuthorBooks)
                .ThenInclude(ab => ab.Author)
                .Include(book => book.Chapters)
                .ThenInclude(chapter => chapter.SourceFile)
                .Include(book => book.SourceFiles)
                .Include(book => book.Images)
                .Where(book => book.Id == id)
                .AsNoTracking()
                .FirstOrDefaultAsync();

            if (null == book)
            {
                return null;
            }

            return BuildAudioBook(book);
        }

        public Task RemoveAsync(AudioBook model)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<AudioBook>> FindAsync(Expression<Func<AudioBook, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public async Task<IReadOnlyList<AudioBook>> QueryBooksAsync(CancellationToken cancellationToken = default)
        {
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
                audioBooks[index] = BuildAudioBook(books[index]);
            }

            return audioBooks;
        }

        private async Task AddAuthorsAsync(Book book, IList<AudioBookAuthor> authors, CancellationToken cancellation)
        {
            for (var index = 0; index < authors.Count; index++)
            {
                var author = authors[index];
                var entity = await context.Authors
                    .Where(a => a.Name == author.Name)
                    .AsNoTracking()
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

        private async Task AddChaptersAsync(Book book, IList<AudioBookChapter> chapters, CancellationToken cancellation)
        {
            for (var chapterIndex = 0; chapterIndex < chapters.Count; chapterIndex++)
            {
                var sourceChapter = chapters[chapterIndex];

                for (var index = 0; index < sourceChapter.Fragments.Count; index++)
                {
                    var sourceFragment = sourceChapter.Fragments[index];
                    var sourceFile = new SourceFile
                    {
                        Filename = sourceFragment.SourceFile.ContentUri,
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

                    book.SourceFiles.Add(sourceFile);
                    book.Chapters.Add(chapter);

                    await context.SourceFiles.AddAsync(sourceFile, cancellation);
                    await context.Chapters.AddAsync(chapter, cancellation);
                }
            }
        }

        private async Task AddCoverImagesAsync(
            Book book,
            IList<AudioBookImage> images,
            CancellationToken cancellationToken)
        {
            for (var imageIndex = 0; imageIndex < images.Count; imageIndex++)
            {
                var sourceImage = images[imageIndex];
                var stream = await sourceImage.GetStreamSync(cancellationToken);
                var contentUri = await coverService.AddImageAsync(stream, cancellationToken);
                var bookImage = new BookImage
                {
                    Book = book,
                    ContentUri = contentUri
                };

                book.Images.Add(bookImage);

                var entry = await context.BookImages.AddAsync(bookImage, cancellationToken);

                if (EntityState.Added == entry.State)
                {
                    ;
                }
            }
        }

        private AudioBook BuildAudioBook(Book source)
        {
            var audioBook = new AudioBook(source.Title, source.Id)
            {
                Synopsis = source.Synopsis
            };

            foreach (var bookAuthor in source.AuthorBooks)
            {
                var author = new AudioBookAuthor(bookAuthor.Author.Name);
                audioBook.Authors.Add(author);
            }

            foreach (var bookChapter in source.Chapters.OrderBy(chapter => chapter.Position))
            {
                var chapter = new AudioBookChapter(audioBook, bookChapter.Title, bookChapter.Offset);
                var sourceFile = GetOrCreateSourceFile(audioBook, bookChapter.SourceFile);
                var fragment = new AudioBookChapterFragment(bookChapter.Offset, bookChapter.Length, sourceFile);

                audioBook.Chapters.Add(chapter);
                chapter.Fragments.Add(fragment);
            }

            foreach (var bookImage in source.Images)
            {
                var image = new ContentProvidedAudioBookImage(audioBook, bookImage.ContentUri, coverService);
                audioBook.Images.Add(image);
            }

            return audioBook;
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

            var source = new AudioBookSourceFile(audioBook, contentUri);

            audioBook.SourceFiles.Add(source);

            return source;
        }
    }
}