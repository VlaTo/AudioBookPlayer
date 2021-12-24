using System;
using AudioBookPlayer.Data.Persistence.Repositories;

namespace AudioBookPlayer.Data.Persistence
{
    public sealed class UnitOfWork : IDisposable
    {
        private readonly LiteDbContext context;
        private bool hasTransaction;

        public IBooksRepository Books
        {
            get;
        }

        public UnitOfWork(LiteDbContext context, bool useTransaction = false)
        {
            this.context = context;

            Books = new BooksRepository(context);

            if (useTransaction)
            {
                hasTransaction = context.BeginTransaction();
            }
        }

        public void Dispose()
        {
            if (hasTransaction)
            {
                context.Rollback();
                hasTransaction = false;
            }
        }

        public void Commit()
        {
            context.Commit();
            hasTransaction = false;
        }
    }
}