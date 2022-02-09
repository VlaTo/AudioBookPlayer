using AudioBookPlayer.Data.Persistence.Entities;
using AudioBookPlayer.Domain;
using LiteDB;

namespace AudioBookPlayer.Data.Persistence
{
    public sealed class LiteDbContext
    {
        private const string filename = "books.db";

        private static LiteDbContext instance;
        private  readonly LiteDatabase database;

        private LiteDbContext(IPathProvider pathProvider)
        {
            var path = pathProvider.GetPath(filename);
            var connectionString = new ConnectionString
            {
                Filename = path
            };

            database = new LiteDatabase(connectionString)
            {
                UserVersion = 1
            };

            var mapper = database.Mapper;

            mapper.Entity<Book>().Id(book => book.Id);
        }

        public static LiteDbContext GetInstance(IPathProvider pathProvider)
        {
            return instance ?? (instance = new LiteDbContext(pathProvider));
        }

        public ILiteCollection<Book> Books() => database.GetCollection<Book>();

        public ILiteCollection<History> History() => database.GetCollection<History>();

        public bool BeginTransaction() => database.BeginTrans();

        public void Rollback() => database.Rollback();

        public void Commit() => database.Commit();
    }
}
