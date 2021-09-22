using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using AudioBookPlayer.App.Domain.Data;
using AudioBookPlayer.App.Domain.Models;

namespace AudioBookPlayer.App.Persistence.SqLite
{
    /*public sealed class BookmarksRepository : IBookmarkRepository
    {
        private readonly ApplicationDbContext context;

        public BookmarksRepository(ApplicationDbContext context)
        {
            this.context = context;
        }

        public Task AddAsync(BookmarkPosition model)
        {
            throw new NotImplementedException();
        }

        public Task<BookmarkPosition> GetAsync(long id)
        {
            throw new NotImplementedException();
        }

        public Task RemoveAsync(BookmarkPosition model)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<BookmarkPosition>> FindAsync(Expression<Func<BookmarkPosition, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<BookmarkPosition>> QueryAsync(long bookId, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }*/
}