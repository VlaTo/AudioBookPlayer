using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using AudioBookPlayer.Data.Persistence.Entities;

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
            throw new NotImplementedException();
        }

        public void Remove(Book entity)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Book> Find(Expression<Func<Book, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public int Count() => context.Books().Count();

        public IEnumerable<Book> All() => context.Books().FindAll();
    }
}