using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace AudioBookPlayer.App.Services
{
    public interface ICoverImageProvider
    {
        Task<Stream> LoadStreamAsync(string contentUri, CancellationToken cancellationToken = default);
    }
}