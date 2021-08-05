using System.Data;
using System.Threading;
using System.Threading.Tasks;
using AudioBookPlayer.App.Domain.Data;
using AudioBookPlayer.App.Domain.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace AudioBookPlayer.App.Persistence
{
    public sealed class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext context;
        private IDbContextTransaction transaction;

        public IBooksRepository Books
        {
            get;
        }

        public IBookmarkRepository Bookmarks
        {
            get;
        }

        public UnitOfWork(
            ApplicationDbContext context,
            ICoverService coverService,
            bool useTransaction)
        {
            this.context = context;

            Books = new BooksRepository(context, coverService);
            Bookmarks = new BookmarksRepository(context);

            if (useTransaction)
            {
                transaction = context.Database.BeginTransaction(IsolationLevel.ReadCommitted);
            }
        }

        public async Task CommitAsync(CancellationToken cancellationToken = default)
        {
            await context.SaveChangesAsync(cancellationToken);

            if (null != transaction)
            {
                await transaction.CommitAsync(cancellationToken);
                transaction = null;
            }
        }

        public void Dispose()
        {
            if (null != transaction)
            {
                transaction.Rollback();
                transaction = null;
            }
        }
    }
}