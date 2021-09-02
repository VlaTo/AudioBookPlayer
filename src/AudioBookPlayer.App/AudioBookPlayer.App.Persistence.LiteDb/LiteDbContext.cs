using System;
using System.Linq.Expressions;
using AudioBookPlayer.App.Persistence.LiteDb.Models;
using LibraProgramming.Xamarin.Core;
using LiteDB;

namespace AudioBookPlayer.App.Persistence.LiteDb
{
    public class LiteDbContext
    {
        private const string databaseFilename = "library.ldb";

        private readonly IPlatformDatabasePath databasePath;
        private LiteDatabase database;

        public LiteDbContext(IPlatformDatabasePath databasePath)
        {
            this.databasePath = databasePath;
        }

        public void Initialize()
        {
            var connectionString = new ConnectionString
            {
                Filename = databasePath.GetDatabasePath(databaseFilename)
            };

            database = new LiteDatabase(connectionString);
            database.UserVersion = 1;

            var mapper = database.Mapper;

            mapper.Entity<Book>().Id(book => book.Id);
        }

        public ILiteCollection<Book> Books() => database.GetCollection<Book>();

        internal bool BeginTransaction() => database.BeginTrans();

        internal bool CommitTransaction() => database.Commit();

        internal bool RollbackTransaction() => database.Rollback();
    }
}
