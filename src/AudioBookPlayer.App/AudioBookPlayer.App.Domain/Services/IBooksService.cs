using System.Threading;
using System.Threading.Tasks;
using AudioBookPlayer.App.Domain.Models;
using AudioBookPlayer.App.Domain.Providers;

namespace AudioBookPlayer.App.Domain.Services
{
    public interface IBooksService : IBooksProvider
    {
        Task<AudioBook> GetBookAsync(long bookId, CancellationToken cancellationToken = default);

        Task SaveBookAsync(AudioBook audioBook, CancellationToken cancellationToken = default);

        AudioBook GetBook(EntityId entityId);
    }
}