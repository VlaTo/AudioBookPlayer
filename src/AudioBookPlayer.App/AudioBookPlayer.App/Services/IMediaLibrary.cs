using AudioBookPlayer.App.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AudioBookPlayer.App.Services
{
    /// <summary>
    /// 
    /// </summary>
    public interface IMediaLibrary : IBooksProvider
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="bookId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<AudioBook> GetBookAsync(long bookId, CancellationToken cancellationToken = default);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="audioBook"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task SaveBookAsync(AudioBook audioBook, CancellationToken cancellationToken = default);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bookId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<IReadOnlyCollection<AudioBookPosition>> QueryBookmarksAsync(long bookId, CancellationToken cancellationToken = default);
    }
}