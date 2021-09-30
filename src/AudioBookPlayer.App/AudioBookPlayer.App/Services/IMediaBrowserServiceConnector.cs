using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AudioBookPlayer.App.Core;
using AudioBookPlayer.App.Domain.Models;

namespace AudioBookPlayer.App.Services
{
    /// <summary>
    /// 
    /// </summary>
    public interface IMediaBrowserServiceConnector
    {
        event EventHandler<PlaybackStateEventArgs> PlaybackStateChanged;

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        Task ConnectAsync();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        Task<IReadOnlyList<BookItem>> GetLibraryAsync();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        Task<BookItem> GetBookItemAsync(EntityId bookId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bookId"></param>
        /// <returns></returns>
        Task<IReadOnlyList<SectionItem>> GetSectionsAsync(EntityId bookId);

        /// <summary>
        /// 
        /// </summary>
        Task UpdateLibraryAsync();

        /// <summary>
        /// 
        /// </summary>
        void Play(EntityId bookId);

        /// <summary>
        /// 
        /// </summary>
        void Pause();
    }
}