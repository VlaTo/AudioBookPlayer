using AudioBookPlayer.App.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Data.Common;
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

        public void EnsureCreated()
        {
            Database.EnsureCreated();
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
                    var folder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                    //var info = Directory.CreateDirectory(folder);
                    // /data/user/0/com.libraprogramming.audiobookplayer.app/files/.local/share/library.db
                    databasePath = Path.Combine(folder, databaseName);

                    break;
                }

                default:
                {
                    throw new NotImplementedException("Platform not supported");
                }
            }

            var builder = new DbConnectionStringBuilder
            {
                {"Filename", databasePath}
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
    }
}
