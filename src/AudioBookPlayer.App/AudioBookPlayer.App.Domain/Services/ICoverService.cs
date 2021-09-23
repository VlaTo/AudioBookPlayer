using AudioBookPlayer.App.Domain.Providers;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace AudioBookPlayer.App.Domain.Services
{
    public interface ICoverService : ICoverProvider
    {
        Task<string> AddImageAsync(Stream stream, CancellationToken cancellationToken = default);
    }
}