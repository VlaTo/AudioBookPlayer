using AudioBookPlayer.App.Domain.Models;
using AudioBookPlayer.App.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AudioBookPlayer.App.Services
{
    public interface ILibraryCallback
    {
        void OnGetBooks(IReadOnlyList<BookItem> books);
    }

    /// <summary>
    /// 
    /// </summary>
    public interface IMediaBrowserServiceConnector : IPlaybackController, IPlaybackQueue
    {
        /// <summary>
        /// 
        /// </summary>
        bool IsConnected { get; }

        /// <summary>
        /// 
        /// </summary>
        IAudioBookMetadata AudioBookMetadata { get; }

        /// <summary>
        /// 
        /// </summary>
        IReadOnlyList<IChapterMetadata> Chapters { get; }


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
        /// <returns></returns>
        Task ConnectAsync();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        IDisposable Subscribe(ILibraryCallback callback);

        /// <summary>
        /// 
        /// </summary>
        Task UpdateLibraryAsync();


        /// <summary>
        /// 
        /// </summary>
        /// <param name="mediaId"></param>
        void PrepareFromMediaId(MediaId mediaId);
    }
}