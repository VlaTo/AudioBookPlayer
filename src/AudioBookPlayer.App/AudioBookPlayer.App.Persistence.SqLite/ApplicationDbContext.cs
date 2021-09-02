using AudioBookPlayer.App.Persistence.SqLite.Models;
using Microsoft.EntityFrameworkCore;

namespace AudioBookPlayer.App.Persistence.SqLite
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

        public abstract DbSet<ChapterFragment> ChapterFragments
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

        public abstract DbSet<Part> Parts
        {
            get;
            set;
        }

        public abstract void Initialize();
    }
}