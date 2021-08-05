using AudioBookPlayer.App.Persistence.Models;
using Microsoft.EntityFrameworkCore;

namespace AudioBookPlayer.App.Persistence
{
    public abstract class ApplicationDbContext : DbContext
    {
        public abstract DbSet<Author> Authors
        {
            get;
            set;
        }

        public abstract DbSet<Book> Books
        {
            get;
            set;
        }

        public abstract DbSet<SourceFile> SourceFiles
        {
            get;
            set;
        }

        public abstract DbSet<AuthorBook> AuthorBooks
        {
            get;
            set;
        }

        public abstract DbSet<BookImage> BookImages
        {
            get;
            set;
        }

        public abstract DbSet<Chapter> Chapters
        {
            get;
            set;
        }

        public abstract void Initialize();
    }
}