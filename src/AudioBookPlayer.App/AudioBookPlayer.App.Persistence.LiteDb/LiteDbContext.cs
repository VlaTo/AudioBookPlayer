using AudioBookPlayer.App.Persistence.LiteDb.Models;
using LibraProgramming.Xamarin.Core;
using LiteDB;

namespace AudioBookPlayer.App.Persistence.LiteDb
{
    public class LiteDbContext
    {
        private const string databaseFilename = "library.ldb";

        private readonly IDatabasePathProvider pathProvider;
        private readonly LiteDatabase database;

        public LiteDbContext(IDatabasePathProvider pathProvider)
        {
            this.pathProvider = pathProvider;

            var connectionString = new ConnectionString
            {
                Filename = pathProvider.GetDatabasePath(databaseFilename)
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
