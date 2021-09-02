using AudioBookPlayer.App.Domain.Models;
using AudioBookPlayer.App.Domain.Services;
using AudioBookPlayer.App.Persistence.LiteDb;
using LibraProgramming.Xamarin.Core;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AudioBookPlayer.App.Services
{
    internal sealed class BooksService : IBooksService
    {
        private readonly LiteDbContext context;
        private readonly ICoverService coverService;
        private readonly ICache<long, AudioBook> cache;

        public BooksService(LiteDbContext context, ICoverService coverService)
        {
            this.context = context;
            this.coverService = coverService;
            cache = new InMemoryCache<long, AudioBook>();
        }

        public async Task<IReadOnlyList<AudioBook>> QueryBooksAsync(CancellationToken cancellationToken = default)
        {
            var books = await QueryBooksInternalAsync(cancellationToken);

            for (var index = 0; index < books.Count; index++)
            {
                var book = books[index];
                cache.Put(book.Id.Value, book);
            }

            return books;
        }

        public async Task<AudioBook> GetBookAsync(long bookId, CancellationToken cancellationToken = default)
        {
            if (cache.Has(bookId))
            {
                return cache.Get(bookId);
            }

            var book = await GetBookInternalAsync(bookId, cancellationToken);

            if (null != book)
            {
                cache.Put(bookId, book);
            }

            return book;
        }

        public async Task SaveBookAsync(AudioBook audioBook, CancellationToken cancellationToken = default)
        {
            await SaveBookInternalAsync(audioBook, cancellationToken);

            cache.Invalidate(audioBook.Id.Value);
        }

        private Task<IReadOnlyList<AudioBook>> QueryBooksInternalAsync(CancellationToken cancellationToken)
        {
            using (var unitOfWork = new UnitOfWork(context, coverService, false))
            {
                return unitOfWork.Books.QueryBooksAsync(cancellationToken);
            }
        }

        private Task<AudioBook> GetBookInternalAsync(long bookId, CancellationToken cancellationToken)
        {
            using (var unitOfWork = new UnitOfWork(context, coverService, false))
            {
                return unitOfWork.Books.GetAsync(bookId);
            }
        }

        private async Task SaveBookInternalAsync(AudioBook audioBook, CancellationToken cancellationToken)
        {
            using (var unitOfWork = new UnitOfWork(context, coverService, false))
            {
                await unitOfWork.Books.AddAsync(audioBook);
                await unitOfWork.CommitAsync(cancellationToken);
            }
        }
    }
}