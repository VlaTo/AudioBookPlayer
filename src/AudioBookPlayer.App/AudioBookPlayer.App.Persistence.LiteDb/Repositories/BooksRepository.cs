using AudioBookPlayer.App.Persistence.LiteDb.Models;
using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace AudioBookPlayer.App.Persistence.LiteDb.Repositories
{
    public sealed partial class BooksRepository : IBooksRepository
    {
        private readonly LiteDbContext context;
        //private readonly ICoverService coverService;

        public BooksRepository(LiteDbContext context/*, ICoverService coverService*/)
        {
            this.context = context;
            // this.coverService = coverService;
        }

        public void Add(Book entity)
        {
            //var builder = BookBuilder.Create(coverService);
            //var book = await builder.MapFromAsync(entity);

            var id = context.Books().Insert(entity);

            //EnsureKeyAssigned(entity, id);
        }

        public Book Get(long id)
        {
            var books = context.Books();
            
            books.EnsureIndex(book => book.Id);
            
            var book = books.FindById(new BsonValue(id));

            if (null == book)
            {
                return null;
            }

            //var builder = AudioBookBuilder.Create(coverService);
            //var entity = builder.MapFrom(book);

            return book;
        }

        public void Remove(Book entity)
        {
            var success = context.Books().Delete(new BsonValue(entity.Id));
        }

        public IEnumerable<Book> Find(Expression<Func<Book, bool>> predicate)
        {
            var books = context.Books().Find(predicate);
            return books;
        }

        /*public Task<IReadOnlyList<AudioBook>> QueryBooksAsync(CancellationToken cancellationToken = default)
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
        }*/

        public IEnumerable<Book> GetAll()
        {
            var books = context.Books().FindAll();
            //var entities = new List<AudioBook>();

            /*foreach (var book in books)
            {
                var builder = AudioBookBuilder.Create(coverService);
                var entity = builder.MapFrom(book);
                entities.Add(entity);
            }*/

            return books;
        }

        /*private static void EnsureKeyAssigned(AudioBook audioBook, BsonValue id)
        {
            if (false == audioBook.Id.HasValue)
            {
                audioBook.Id = id.AsInt64;
            }
        }*/
    }
}