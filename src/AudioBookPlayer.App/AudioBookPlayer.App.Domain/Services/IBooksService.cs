using System.Threading;
using System.Threading.Tasks;
using AudioBookPlayer.App.Domain.Data;
using AudioBookPlayer.App.Domain.Models;

namespace AudioBookPlayer.App.Domain.Services
{
    public interface IBooksService : IBooksProvider
    {
        Task<AudioBook> GetAudioBookAsync(long bookId, CancellationToken cancellationToken = default);
    }
}