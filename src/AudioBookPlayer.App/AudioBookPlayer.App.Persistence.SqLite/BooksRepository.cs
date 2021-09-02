using AudioBookPlayer.App.Domain.Data;
using AudioBookPlayer.App.Domain.Extensions;
using AudioBookPlayer.App.Domain.Models;
using AudioBookPlayer.App.Domain.Services;
using AudioBookPlayer.App.Persistence.SqLite.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace AudioBookPlayer.App.Persistence.SqLite
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
                ChapterFragments = new List<ChapterFragment>(),
                Images = new List<BookImage>(),
                Parts = new List<Part>(),
                Chapters = new List<Chapter>()
            };

            EnsureKeyAssigned(await context.Books.AddAsync(book));

            await AddAuthorsAsync(book, model.Authors, CancellationToken.None);
            await AddPartsAsync(book, model.Sections, CancellationToken.None);
            await AddChaptersAsync(book, model.Chapters, CancellationToken.None);
            await AddCoverImagesAsync(book, model.Images, CancellationToken.None);

            model.Id = book.Id;
        }

        public async Task<AudioBook> GetAsync(long id)
        {
            var book = await context.Books
                .Include(book => book.AuthorBooks)
                .ThenInclude(ab => ab.Author)
                .Include(book => book.Parts)
                .ThenInclude(part => part.Chapters)
                .Include(book => book.Chapters)
                .ThenInclude(chapter => chapter.ChapterFragments)
                .Include(book => book.Chapters)
                .ThenInclude(chapter => chapter.Part)
                .Include(book => book.ChapterFragments)
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
                .Include(book => book.Parts)
                .ThenInclude(part => part.Chapters)
                .Include(book => book.Chapters)
                .ThenInclude(chapter => chapter.ChapterFragments)
                .Include(book => book.Chapters)
                .ThenInclude(chapter => chapter.Part)
                .Include(book => book.ChapterFragments)
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

        private async Task AddPartsAsync(Book book, IList<AudioBookSection> parts, CancellationToken cancellationToken)
        {
            for (var partIndex = 0; partIndex < parts.Count; partIndex++)
            {
                var source = parts[partIndex];
                var part = new Part
                {
                    Book = book,
                    Title = source.Title,
                    Chapters = new List<Chapter>()
                };

                book.Parts.Add(part);

                EnsureKeyAssigned(await context.Parts.AddAsync(part, cancellationToken));
            }
        }

        private async Task AddChaptersAsync(Book book, IList<AudioBookChapter> chapters, CancellationToken cancellation)
        {
            for (var chapterIndex = 0; chapterIndex < chapters.Count; chapterIndex++)
            {
                var sourceChapter = chapters[chapterIndex];
                var chapter = new Chapter
                {
                    Position = chapterIndex,
                    Title = sourceChapter.Title,
                    Start = sourceChapter.Start,
                    Length = sourceChapter.Duration,
                    Book = book,
                    ChapterFragments = new List<ChapterFragment>()
                };

                if (null != sourceChapter.Section)
                {
                    var part = GetPart(book, sourceChapter.Section.Title);

                    if (null != part)
                    {
                        part.Chapters.Add(chapter);
                    }
                }

                book.Chapters.Add(chapter);

                EnsureKeyAssigned(await context.Chapters.AddAsync(chapter, cancellation));

                for (var index = 0; index < sourceChapter.Fragments.Count; index++)
                {
                    var sourceFragment = sourceChapter.Fragments[index];
                    var fragment = new ChapterFragment
                    {
                        ContentUri = sourceFragment.SourceFile.ContentUri,
                        Start = sourceFragment.Start,
                        Length = sourceFragment.Duration,
                        Book = book
                    };

                    chapter.ChapterFragments.Add(fragment);
                    book.ChapterFragments.Add(fragment);

                    EnsureKeyAssigned(await context.ChapterFragments.AddAsync(fragment, cancellation));
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
                string contentUri;
                
                await using (var stream = await sourceImage.GetStreamAsync(cancellationToken))
                {
                    contentUri = await coverService.AddImageAsync(stream, cancellationToken);
                }

                var bookImage = new BookImage
                {
                    Book = book,
                    ContentUri = contentUri
                };

                book.Images.Add(bookImage);

                EnsureKeyAssigned(await context.BookImages.AddAsync(bookImage, cancellationToken));
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
                var part = audioBook.GetOrCreatePart(bookChapter.Part?.Title);
                var chapter = new AudioBookChapter(audioBook, bookChapter.Title, bookChapter.Start, part);

                if (null != part)
                {
                    part.Chapters.Add(chapter);
                }

                foreach (var chapterFragment in bookChapter.ChapterFragments)
                {
                    var sourceFile = GetOrCreateSourceFile(audioBook, chapterFragment);
                    var fragment = new AudioBookChapterFragment(chapterFragment.Start, chapterFragment.Length, sourceFile);
                    chapter.Fragments.Add(fragment);
                }

                audioBook.Chapters.Add(chapter);
            }

            foreach (var bookImage in source.Images)
            {
                var image = new ContentProvidedAudioBookImage(audioBook, bookImage.ContentUri, coverService);
                audioBook.Images.Add(image);
            }

            return audioBook;
        }

        private static AudioBookSourceFile GetOrCreateSourceFile(AudioBook audioBook, ChapterFragment chapterFragment)
        {
            var contentUri = chapterFragment.ContentUri;

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

        private static Part GetPart(Book book, string title)
        {
            return book.Parts.FirstOrDefault(
                part => String.Equals(part.Title, title, StringComparison.InvariantCulture)
            );
        }

        private static void EnsureKeyAssigned<T>(EntityEntry<T> entry)
            where T : class
        {
            if (null == entry)
            {
                throw new Exception();
            }

            /*if (EntityState.Added == entry.State && entry.IsKeySet)
            {
                return;
            }

            throw new Exception();*/
        }
    }
}