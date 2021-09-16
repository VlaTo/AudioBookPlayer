using AudioBookPlayer.App.Domain.Data;
using AudioBookPlayer.App.Domain.Models;
using AudioBookPlayer.App.Domain.Services;
using LiteDB;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace AudioBookPlayer.App.Persistence.LiteDb.Repositories
{
    public sealed partial class BooksRepository : IBooksRepository
    {
        private readonly LiteDbContext context;
        private readonly ICoverService coverService;

        public BooksRepository(LiteDbContext context, ICoverService coverService)
        {
            this.context = context;
            this.coverService = coverService;
        }

        public async Task AddAsync(AudioBook entity)
        {
            var builder = BookBuilder.Create(coverService);
            var book = await builder.MapFromAsync(entity);

            EnsureKeyAssigned(entity, context.Books().Insert(book));
        }

        public Task<AudioBook> GetAsync(long id)
        {
            var books = context.Books();
            
            books.EnsureIndex(book => book.Id);
            
            var book = books.FindById(new BsonValue(id));

            if (null == book)
            {
                return Task.FromResult<AudioBook>(null);
            }

            var builder = AudioBookBuilder.Create(coverService);
            var entity = builder.MapFrom(book);

            return Task.FromResult(entity);
        }

        public Task RemoveAsync(AudioBook entity)
        {
            var success = context.Books().Delete(new BsonValue(entity.Id));
            return Task.CompletedTask;
        }

        public Task<IEnumerable<AudioBook>> FindAsync(Expression<Func<AudioBook, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public Task<IReadOnlyList<AudioBook>> QueryBooksAsync(CancellationToken cancellationToken = default)
        {
            var books = context.Books().FindAll();
            var entities = new List<AudioBook>();

            foreach (var book in books)
            {
                var builder = AudioBookBuilder.Create(coverService);
                var entity = builder.MapFrom(book);
                entities.Add(entity);
            }

            return Task.FromResult<IReadOnlyList<AudioBook>>(new ReadOnlyCollection<AudioBook>(entities));
        }

        public IReadOnlyList<AudioBook> QueryBooks()
        {
            var books = context.Books().FindAll();
            var entities = new List<AudioBook>();

            foreach (var book in books)
            {
                var builder = AudioBookBuilder.Create(coverService);
                var entity = builder.MapFrom(book);
                entities.Add(entity);
            }

            return new ReadOnlyCollection<AudioBook>(entities);
        }

        private static void EnsureKeyAssigned(AudioBook audioBook, BsonValue id)
        {
            if (false == audioBook.Id.HasValue)
            {
                audioBook.Id = id.AsInt64;
            }
        }
    }
}