using AudioBookPlayer.App.Domain.Data;
using AudioBookPlayer.App.Domain.Services;
using AudioBookPlayer.App.Persistence.LiteDb.Repositories;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AudioBookPlayer.App.Persistence.LiteDb
{
    public sealed class UnitOfWork : IUnitOfWork
    {
        private readonly LiteDbContext context;
        private bool hasTransaction;

        public IBooksRepository Books { get; }

        public IBookmarkRepository Bookmarks { get; }

        public UnitOfWork(LiteDbContext context, ICoverService coverService, bool useTransaction)
        {
            this.context = context;

            Books = new BooksRepository(context, coverService);

            if (useTransaction)
            {
                hasTransaction = context.BeginTransaction();
            }
        }

        public void Dispose()
        {
            if (hasTransaction)
            {
                Rollback();
            }
        }

        public Task CommitAsync(CancellationToken cancellationToken = default)
        {
            if (hasTransaction)
            {
                if (context.CommitTransaction())
                {
                    hasTransaction = false;
                    return Task.CompletedTask;
                }

                throw new Exception();
            }

            return Task.CompletedTask;
        }

        private void Rollback()
        {
            if (hasTransaction)
            {
                if (context.RollbackTransaction())
                {
                    hasTransaction = false;
                    return;
                }

                throw new Exception();
            }
        }
    }
}