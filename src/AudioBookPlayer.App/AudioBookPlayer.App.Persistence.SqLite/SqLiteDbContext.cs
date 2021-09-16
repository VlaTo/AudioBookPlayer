using AudioBookPlayer.App.Persistence.SqLite.Models;
using LibraProgramming.Xamarin.Core;
using LibraProgramming.Xamarin.Dependency.Container.Attributes;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Xamarin.Forms;

namespace AudioBookPlayer.App.Persistence.SqLite
{
    public class SqLiteDbContext : ApplicationDbContext
    {
        private const string DatabaseName = "library.db";

        private readonly IDatabasePathProvider pathProviderProvider;

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

        public sealed override DbSet<ChapterFragment> ChapterFragments
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
        
        public sealed override DbSet<Part> Parts
        {
            get;
            set;
        }

        [PrefferedConstructor]
        protected SqLiteDbContext(IDatabasePathProvider pathProviderProvider)
        {
            this.pathProviderProvider = pathProviderProvider;
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
            var databasePath = pathProviderProvider.GetDatabasePath(DatabaseName);

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
            // ChapterFragments
            modelBuilder.Entity<ChapterFragment>()
                .HasIndex(chapterFragment => new {chapterFragment.BookId, chapterFragment.ChapterId})
                .IsUnique();

            // BookImages
            modelBuilder.Entity<BookImage>()
                .HasIndex(bookImage => new {bookImage.Id, bookImage.BookId})
                .IsUnique();

            // Parts
            modelBuilder.Entity<Part>()
                .HasIndex(part => new { part.Id, part.BookId })
                .IsUnique();

            // Chapters
            modelBuilder.Entity<Chapter>()
                .HasIndex(chapter => new {chapter.Id, chapter.BookId})
                .IsUnique();

            modelBuilder.Entity<Chapter>()
                .HasIndex(chapter => new {chapter.Position});

            // AuthorBooks
            modelBuilder.Entity<AuthorBook>()
                .HasKey(authorBook => new {authorBook.AuthorId, authorBook.BookId});

            modelBuilder.Entity<AuthorBook>()
                .HasOne(authorBook => authorBook.Book)
                .WithMany(book => book.AuthorBooks)
                .HasForeignKey(authorBook => authorBook.BookId);

            modelBuilder.Entity<AuthorBook>()
                .HasOne(authorBook => authorBook.Author)
                .WithMany(author => author.AuthorBooks)
                .HasForeignKey(authorBook => authorBook.AuthorId);
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
