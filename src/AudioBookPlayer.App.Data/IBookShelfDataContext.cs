using AudioBookPlayer.App.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace AudioBookPlayer.App.Data
{
    public interface IBookShelfDataContext
    {
        DbSet<Book> Books
        {
            get;
        }

        DbSet<SourceFile> SourceFiles
        {
            get;
        }

        void Initialize();
    }
}