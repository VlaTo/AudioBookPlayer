using AudioBookPlayer.App.Persistence.LiteDb.Models;
using LibraProgramming.Xamarin.Core;
using LiteDB;

namespace AudioBookPlayer.App.Persistence.LiteDb
{
    public class LiteDbContext
    {
        private const string DatabaseFilename = "library.ldb";

        private readonly LiteDatabase database;

        public LiteDbContext(IDatabasePathProvider provider)
        {
            var connectionString = new ConnectionString
            {
                Filename = provider.GetDatabasePath(DatabaseFilename)
            };

            database = new LiteDatabase(connectionString);
            database.UserVersion = 1;

            var mapper = database.Mapper;

            mapper.Entity<Book>().Id(book => book.Id);
        }

        public ILiteCollection<Book> Books() => database.GetCollection<Book>();

        public ILiteCollection<Activity> Activities() => database.GetCollection<Activity>();

        internal bool BeginTransaction() => database.BeginTrans();

        internal bool CommitTransaction() => database.Commit();

        internal bool RollbackTransaction() => database.Rollback();
    }
}
