using AudioBookPlayer.App.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Threading;
using System.Threading.Tasks;

namespace AudioBookPlayer.App.Data
{
    public interface IBookShelfDataContext
    {
        DbSet<Author> Authors
        {
            get;
        }

        DbSet<Book> Books
        {
            get;
        }
        DbSet<AuthorBook> AuthorBooks
        {
            get;
        }

        DbSet<SourceFile> SourceFiles
        {
            get;
        }
        
        DbSet<BookImage> BookImages
        {
            get;
        }

        DbSet<Chapter> Chapters
        {
            get;
        }

        void Initialize();

        Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default);

        Task<bool> SaveChangesAsync(CancellationToken cancellation = default);

        Task DeleteAllAsync(CancellationToken cancellation = default);
    }
}