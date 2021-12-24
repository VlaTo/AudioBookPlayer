using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using AudioBookPlayer.Data.Persistence.Entities;

namespace AudioBookPlayer.Data.Persistence.Repositories
{
    public class BooksRepository : IBooksRepository
    {
        private readonly LiteDbContext context;

        public BooksRepository(LiteDbContext context)
        {
            this.context = context;
        }

        public void Add(Book entity)
        {
            throw new NotImplementedException();
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