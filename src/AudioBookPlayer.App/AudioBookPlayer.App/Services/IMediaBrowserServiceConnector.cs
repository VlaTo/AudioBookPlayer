using System;
using System.Reactive;
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
        IObservable<Unit> Connected
        {
            get;
        }

        /// <summary>
        /// 
        /// </summary>
        void Connect();

        IObservable<AudioBook> GetRoot();
    }
}