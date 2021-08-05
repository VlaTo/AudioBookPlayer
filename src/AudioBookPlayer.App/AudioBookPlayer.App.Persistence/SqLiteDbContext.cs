using AudioBookPlayer.App.Persistence.Models;
using LibraProgramming.Xamarin.Core;
using LibraProgramming.Xamarin.Dependency.Container.Attributes;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Xamarin.Forms;

namespace AudioBookPlayer.App.Persistence
{
    public class SqLiteDbContext : ApplicationDbContext
    {
        private const string DatabaseName = "library.db";

        private readonly IPlatformDatabasePath pathProvider;

        public sealed override DbSet<Author> Authors
        {
            get;
            set;
        }

        public sealed override DbSet<Book> Books
        {
            get;
            set;
        }

        public sealed override DbSet<SourceFile> SourceFiles
        {
            get;
            set;
        }

        public sealed override DbSet<AuthorBook> AuthorBooks
        {
            get;
            set;
        }

        public sealed override DbSet<BookImage> BookImages
        {
            get;
            set;
        }

        public sealed override DbSet<Chapter> Chapters
        {
            get;
            set;
        }

        [PrefferedConstructor]
        protected SqLiteDbContext(IPlatformDatabasePath pathProvider)
        {
            this.pathProvider = pathProvider;
        }

        public override void Initialize()
        {
            //Database.EnsureDeleted();
            Database.EnsureCreated();
            //CreateMigrationHistory();
            //Database.Migrate();
        }

        /*public Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default)
        {
            return Database.BeginTransactionAsync(cancellationToken);
        }

        async Task<bool> IMediaLibraryDataContext.SaveChangesAsync(CancellationToken cancellation)
        {
            return 0 < await base.SaveChangesAsync(cancellation);
        }*/

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var databasePath = pathProvider.GetDatabasePath(DatabaseName);

            switch (Device.RuntimePlatform)
            {
                case Device.iOS:
                {
                    SQLitePCL.Batteries_V2.Init();
                    break;
                }
            }

            var builder = new SqliteConnectionStringBuilder
            {
                DataSource = databasePath,
                Cache = SqliteCacheMode.Private,
                Mode = SqliteOpenMode.ReadWriteCreate
            };
            
            optionsBuilder.UseSqlite(builder.ConnectionString);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // SourceFiles
            modelBuilder.Entity<SourceFile>()
                .HasIndex(sf => new {sf.Id, sf.BookId})
                .IsUnique();

            // BookImages
            modelBuilder.Entity<BookImage>()
                .HasIndex(bi => new {bi.Id, bi.BookId})
                .IsUnique();

            // Chapters
            modelBuilder.Entity<Chapter>()
                .HasIndex(bi => new {bi.Id, bi.BookId})
                .IsUnique();

            modelBuilder.Entity<Chapter>()
                .HasIndex(bi => new {bi.Position});

            // AuthorBooks
            modelBuilder.Entity<AuthorBook>()
                .HasKey(ab => new {ab.AuthorId, ab.BookId});

            modelBuilder.Entity<AuthorBook>()
                .HasOne(ab => ab.Book)
                .WithMany(b => b.AuthorBooks)
                .HasForeignKey(ab => ab.BookId);

            modelBuilder.Entity<AuthorBook>()
                .HasOne(ab => ab.Author)
                .WithMany(a => a.AuthorBooks)
                .HasForeignKey(ab => ab.AuthorId);
        }
        
        /*private void CreateMigrationHistory()
        {
            using (var transaction = Database.BeginTransaction())
            {
                var sql = "CREATE TABLE [__EFMigrationsHistory] ([MigrationId] text NOT NULL, [ProductVersion] text NOT NULL, CONSTRAINT [sqlite_autoindex___EFMigrationsHistory_1] PRIMARY KEY ([MigrationId]));";
                var result = Database.ExecuteSqlRaw(sql);

                transaction.Commit();
            }
        }*/
    }
}
