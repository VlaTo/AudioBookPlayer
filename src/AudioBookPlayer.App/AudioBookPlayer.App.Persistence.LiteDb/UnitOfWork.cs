using AudioBookPlayer.App.Persistence.LiteDb.Repositories;
using System;

namespace AudioBookPlayer.App.Persistence.LiteDb
{
    public sealed class UnitOfWork : IUnitOfWork
    {
        private readonly LiteDbContext context;
        private bool hasTransaction;

        public IBooksRepository Books
        {
            get;
        }

        public UnitOfWork(LiteDbContext context, bool useTransaction)
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
                Rollback();
            }
        }

        public void Commit()
        {
            if (false == hasTransaction)
            {
                return;
            }

            if (false == context.CommitTransaction())
            {
                throw new Exception();
            }

            hasTransaction = false;
        }

        private void Rollback()
        {
            if (false == hasTransaction)
            {
                return;
            }

            if (false == context.RollbackTransaction())
            {
                throw new Exception();
            }

            hasTransaction = false;
        }
    }
}