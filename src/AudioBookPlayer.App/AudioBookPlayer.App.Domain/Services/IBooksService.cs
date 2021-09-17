using System.Threading;
using System.Threading.Tasks;
using AudioBookPlayer.App.Domain.Data;
using AudioBookPlayer.App.Domain.Models;

namespace AudioBookPlayer.App.Domain.Services
{
    public interface IBooksService : IBooksProvider
    {
        Task<AudioBook> GetBookAsync(long bookId, CancellationToken cancellationToken = default);

        Task SaveBookAsync(AudioBook audioBook, CancellationToken cancellationToken = default);

        AudioBook GetBook(BookId bookId);
    }
}