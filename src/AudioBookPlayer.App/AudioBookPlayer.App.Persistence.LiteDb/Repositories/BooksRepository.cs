using AudioBookPlayer.App.Persistence.LiteDb.Models;
using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace AudioBookPlayer.App.Persistence.LiteDb.Repositories
{
    public sealed partial class BooksRepository : IBooksRepository
    {
        private readonly LiteDbContext context;

        public BooksRepository(LiteDbContext context)
        {
            this.context = context;
        }

        public void Add(Book entity)
        {
            var id = context.Books().Insert(entity);
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

        public IReadOnlyCollection<Book> GetAll()
        {
            var books = context.Books().FindAll();
            return books.ToArray();
        }

        public int Count() => context.Books().Count();
    }
}