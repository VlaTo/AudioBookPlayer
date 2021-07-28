using System;
using AudioBookPlayer.App.Domain.Models;

namespace AudioBookPlayer.App.Services
{
    /// <summary>
    /// 
    /// </summary>
    public interface IPlaybackService
    {
        /// <summary>
        /// 
        /// </summary>
        AudioBook AudioBook
        {
            get; 
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        int ChapterIndex
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        TimeSpan CurrentPosition
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        bool IsPlaying
        {
            get;
        }

        /// <summary>
        /// 
        /// </summary>
        event EventHandler IsPlayingChanged;

        /// <summary>
        /// 
        /// </summary>
        event EventHandler AudioBookChanged;

        /// <summary>
        /// 
        /// </summary>
        event EventHandler ChapterIndexChanged;

        /// <summary>
        /// 
        /// </summary>
        event EventHandler CurrentPositionChanged;

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
        /// <returns></returns>
        IDisposable BatchUpdate();
    }
}