using System;
using System.Threading;
using System.Threading.Tasks;
using AudioBookPlayer.App.Models;

namespace AudioBookPlayer.App.Services
{
    /// <summary>
    /// 
    /// </summary>
    public interface IPlaybackController
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="audioBook"></param>
        /// <param name="cancellationToken"></param>
        Task<IPlayback> CreatePlaybackAsync(AudioBook audioBook, CancellationToken cancellationToken = default);
    }
}