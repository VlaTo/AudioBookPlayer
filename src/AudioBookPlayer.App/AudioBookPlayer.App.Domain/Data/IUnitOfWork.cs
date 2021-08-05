using System;
using System.Threading;
using System.Threading.Tasks;

namespace AudioBookPlayer.App.Domain.Data
{
    public interface IUnitOfWork : IDisposable
    {
        IBooksRepository Books
        {
            get;
        }

        IBookmarkRepository Bookmarks
        {
            get;
        }

        Task CommitAsync(CancellationToken cancellationToken = default);
    }
}