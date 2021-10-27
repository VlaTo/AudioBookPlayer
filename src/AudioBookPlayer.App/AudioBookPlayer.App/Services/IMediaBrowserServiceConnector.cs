using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AudioBookPlayer.App.Core;
using AudioBookPlayer.App.Domain.Models;
using AudioBookPlayer.App.Models;

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
        bool IsConnected
        {
            get;
        }

        /// <summary>
        /// 
        /// </summary>
        long ActiveQueueItemId
        {
            get;
        }

        /// <summary>
        /// 
        /// </summary>
        /*int QueueIndex
        {
            get;
        }*/

        /// <summary>
        /// 
        /// </summary>
        long PlaybackPosition
        {
            get;
        }

        /// <summary>
        /// 
        /// </summary>
        long PlaybackDuration
        {
            get;
        }

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
        IAudioBookMetadata AudioBookMetadata
        {
            get;
        }

        /// <summary>
        /// 
        /// </summary>
        IReadOnlyList<IChapterMetadata> Chapters
        {
            get;
        }

        /// <summary>
        /// 
        /// </summary>
        event EventHandler PlaybackStateChanged;

        /// <summary>
        /// 
        /// </summary>
        event EventHandler AudioBookMetadataChanged;

        /// <summary>
        /// 
        /// </summary>
        event EventHandler ChaptersChanged;

        /// <summary>
        /// 
        /// </summary>
        event EventHandler QueueIndexChanged;

        /// <summary>
        /// 
        /// </summary>
        event EventHandler CurrentMediaInfoChanged;

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
        void Play();

        /// <summary>
        /// 
        /// </summary>
        void Pause();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mediaId"></param>
        void PrepareFromMediaId(MediaId mediaId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="queueItemId"></param>
        void SetActiveQueueItemId(long queueItemId);

        /// <summary>
        /// 
        /// </summary>
        void SkipToPrevious();

        /// <summary>
        /// 
        /// </summary>
        void SkipToNext();
    }
}