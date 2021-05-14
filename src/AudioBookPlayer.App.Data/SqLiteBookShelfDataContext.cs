using AudioBookPlayer.App.Data.Models;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Storage;
using Xamarin.Forms;

namespace AudioBookPlayer.App.Data
{
    public class SqLiteBookShelfDataContext : DbContext, IBookShelfDataContext
    {
        private const string databaseName = "library.db";

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

        public void Initialize()
        {
            Database.EnsureCreated();
            //CreateMigrationHistory();
            Database.Migrate();
        }

        public Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default) =>
            Database.BeginTransactionAsync(cancellationToken);

        async Task<bool> IBookShelfDataContext.SaveChangesAsync(CancellationToken cancellation) =>
            0 < await base.SaveChangesAsync(cancellation);

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //base.OnConfiguring(optionsBuilder);

            var databasePath = String.Empty;

            switch (Device.RuntimePlatform)
            {
                case Device.iOS:
                {
                    SQLitePCL.Batteries_V2.Init();

                    databasePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "..", "Library", databaseName);

                    break;
                }

                case Device.Android:
                {
                    // /storage/emulated/0/Android/data/com.libraprogramming.audiobookplayer.app/files
                    //var folder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                    var folder = "/storage/emulated/0/Android/data/com.libraprogramming.audiobookplayer.app/files";

                    /*var folders = new[]
                    {
                        Environment.SpecialFolder.LocalApplicationData,
                        Environment.SpecialFolder.ApplicationData,
                        Environment.SpecialFolder.CommonApplicationData,
                        Environment.SpecialFolder.CommonDocuments,
                        Environment.SpecialFolder.MyDocuments,
                        Environment.SpecialFolder.Personal,
                        Environment.SpecialFolder.UserProfile
                    };

                    foreach (var folder in folders)
                    {
                        var path = Environment.GetFolderPath(folder);
                        Debug.Print($"[SqLiteBookShelfDataContext] [OnConfiguring] Folder: {folder} => \"{path}\"");
                    }*/

                    var info = Directory.CreateDirectory(folder);
                    // LocalApplicationData - /data/user/0/com.libraprogramming.audiobookplayer.app/files/.local/share/library.db
                    // ApplicationData - /data/user/0/com.libraprogramming.audiobookplayer.app/files/.config/library.db

                    databasePath = Path.Combine(folder, databaseName);
                    //databasePath = ":memory:";

                    break;
                }

                default:
                {
                    throw new NotImplementedException("Platform not supported");
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
            modelBuilder.Entity<SourceFile>()
                .HasIndex(sf => new {sf.Id, sf.BookId})
                .IsUnique();

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
