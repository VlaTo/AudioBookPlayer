using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using AudioBookPlayer.Data.Persistence.Entities;
using LiteDB;

namespace AudioBookPlayer.Data.Persistence.Repositories
{
    internal class BooksRepository : IBooksRepository
    {
        private readonly LiteDbContext context;

        public BooksRepository(LiteDbContext context)
        {
            this.context = context;
        }

        public void Add(Book entity)
        {
            var collection = context.Books();
            collection.Insert(entity);
        }

        public Book Get(long key)
        {
            var collection = context.Books();
            return collection.FindById(new BsonValue(key));
        }

        public bool Remove(Book entity)
        {
            var collection = context.Books();
            return collection.Delete(new BsonValue(entity.Id));
        }

        public bool Remove(long key)
        {
            var collection = context.Books();
            return collection.Delete(new BsonValue(key));
        }

        public IEnumerable<Book> Find(Expression<Func<Book, bool>> predicate)
        {
            var collection = context.Books();
            return collection.Find(predicate);
        }

        public bool Update(long key, Book book)
        {
            var collection = context.Books();
            return collection.Update(new BsonValue(key), book);
        }

        public IEnumerable<Book> All() => context.Books().FindAll();
    }
}