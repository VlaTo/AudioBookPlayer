using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace AudioBookPlayer.App.Domain.Providers
{
    /// <summary>
    /// 
    /// </summary>
    public interface ICoverProvider
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="contentUri"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<Stream> GetImageAsync(string contentUri, CancellationToken cancellationToken = default);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="contentUri"></param>
        /// <returns></returns>
        Stream GetImage(string contentUri);
    }
}