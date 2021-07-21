using AudioBookPlayer.App.Data.Models;
using LibraProgramming.Xamarin.Core;
using LibraProgramming.Xamarin.Dependency.Container.Attributes;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace AudioBookPlayer.App.Data
{
    public class SqLiteMediaLibraryDataContext : DbContext, IMediaLibraryDataContext
    {
        private const string databaseName = "library.db";

        private readonly IPlatformDatabasePath pathProvider;

        public DbSet<Author> Authors
        {
            get;
            set;
        }

        public DbSet<Book> Books
        {
            get;
            set;
        }

        public DbSet<SourceFile> SourceFiles
        {
            get;
            set;
        }

        public DbSet<AuthorBook> AuthorBooks
        {
            get;
            set;
        }

        public DbSet<BookImage> BookImages
        {
            get;
            set;
        }

        public DbSet<Chapter> Chapters
        {
            get;
            set;
        }

        [PrefferedConstructor]
        protected SqLiteMediaLibraryDataContext(IPlatformDatabasePath pathProvider)
        {
            this.pathProvider = pathProvider;
        }

        public SqLiteMediaLibraryDataContext(DbContextOptions options)
            : base(options)
        {
        }

        public void Initialize()
        {
            //Database.EnsureDeleted();
            Database.EnsureCreated();
            //CreateMigrationHistory();
            //Database.Migrate();
        }

        public Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default)
        {
            return Database.BeginTransactionAsync(cancellationToken);
        }

        async Task<bool> IMediaLibraryDataContext.SaveChangesAsync(CancellationToken cancellation)
        {
            return 0 < await base.SaveChangesAsync(cancellation);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var databasePath = pathProvider.GetDatabasePath(databaseName);

            switch (Device.RuntimePlatform)
            {
                case Device.iOS:
                {
                    SQLitePCL.Batteries_V2.Init();

                    //databasePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "..", "Library", databaseName);

                    break;
                }

                /*case Device.Android:
                {
                    // /storage/emulated/0/Android/data/com.libraprogramming.audiobookplayer.app/files
                    //var folder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                    var folder = "/storage/emulated/0/Android/data/com.libraprogramming.audiobookplayer.app/files";
                    // LocalApplicationData - /data/user/0/com.libraprogramming.audiobookplayer.app/files/.local/share/library.db
                    // ApplicationData - /data/user/0/com.libraprogramming.audiobookplayer.app/files/.config/library.db

                    databasePath = Path.Combine(folder, databaseName);
                    //databasePath = ":memory:";

                    break;
                }

                default:
                {
                    throw new NotImplementedException("Platform not supported");
                }*/
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
        
        private void CreateMigrationHistory()
        {
            using (var transaction = Database.BeginTransaction())
            {
                var sql = "CREATE TABLE [__EFMigrationsHistory] ([MigrationId] text NOT NULL, [ProductVersion] text NOT NULL, CONSTRAINT [sqlite_autoindex___EFMigrationsHistory_1] PRIMARY KEY ([MigrationId]));";
                var result = Database.ExecuteSqlRaw(sql);

                transaction.Commit();
            }
        }
    }
}
