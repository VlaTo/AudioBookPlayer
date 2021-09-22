using System;
using System.Collections.Generic;
using System.Reactive;
using System.Threading.Tasks;
using AudioBookPlayer.App.Domain.Models;
using AudioBookPlayer.App.Domain.Services;
using AudioBookPlayer.App.ViewModels;

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
        IObservable<Unit> Connected
        {
            get;
        }

        /// <summary>
        /// 
        /// </summary>
        IObservable<BookPreviewViewModel[]> Library
        {
            get;
        }

        /// <summary>
        /// 
        /// </summary>
        void Connect();


        /// <summary>
        /// 
        /// </summary>
        void Refresh();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        Task ConnectAsync();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        Task<IReadOnlyList<BookPreviewViewModel>> GetLibraryAsync(ICoverService coverService);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="coverService"></param>
        /// <returns></returns>
        Task<CurrentBookViewModel> GetBookAsync(BookId id, ICoverService coverService);
    }
}