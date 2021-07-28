using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AudioBookPlayer.App.Domain.Models;

namespace AudioBookPlayer.App.Domain.Services
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
        Task<IReadOnlyCollection<NamedAudioBookPosition>> QueryBookmarksAsync(long bookId, CancellationToken cancellationToken = default);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bookId"></param>
        /// <param name="activityType"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task RecordBookActivityAsync(long bookId, AudioBookActivityType activityType, CancellationToken cancellationToken = default);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bookId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<AudioBookActivity> GetLatestBookActivityAsync(long bookId, CancellationToken cancellationToken = default);
    }
}