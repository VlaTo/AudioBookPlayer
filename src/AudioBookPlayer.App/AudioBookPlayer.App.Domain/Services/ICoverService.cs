using AudioBookPlayer.App.Domain.Providers;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace AudioBookPlayer.App.Domain.Services
{
    /// <summary>
    /// 
    /// </summary>
    public interface ICoverService : ICoverProvider
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<string> AddImageAsync(Stream stream, CancellationToken cancellationToken = default);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        string AddImage(Stream stream);
    }
}