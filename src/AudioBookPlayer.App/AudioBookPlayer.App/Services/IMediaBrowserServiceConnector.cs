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
        /// <summary>
        /// 
        /// </summary>
        PlaybackState PlaybackState
        {
            get;
        }

        /// <summary>
        /// 
        /// </summary>
        IMediaMetadataDescription MediaMetadataDescription
        {
            get;
        }

        IReadOnlyCollection<>

        /// <summary>
        /// 
        /// </summary>
        event EventHandler PlaybackStateChanged;

        /// <summary>
        /// 
        /// </summary>
        event EventHandler MediaMetadataChanged;

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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mediaId"></param>
        void PrepareFromMediaId(MediaId mediaId);
    }
}