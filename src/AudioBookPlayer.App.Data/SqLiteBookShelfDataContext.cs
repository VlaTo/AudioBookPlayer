using AudioBookPlayer.App.Data.Models;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using Xamarin.Forms;

namespace AudioBookPlayer.App.Data
{
    public class SqLiteBookShelfDataContext : DbContext, IBookShelfDataContext
    {
        private const string databaseName = "library.db";

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

        public void Initialize()
        {
            Database.EnsureCreated();
            //CreateMigrationHistory();
            //Database.Migrate();
        }

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
            //base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<SourceFile>()
                .HasIndex(nameof(SourceFile.Id), nameof(SourceFile.BookId))
                .IsUnique();
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
