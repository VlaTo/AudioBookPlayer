using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace AudioBookPlayer.App.Domain.Providers
{
    public interface ICoverProvider
    {
        Task<Stream> GetImageAsync(string contentUri, CancellationToken cancellationToken = default);
    }
}