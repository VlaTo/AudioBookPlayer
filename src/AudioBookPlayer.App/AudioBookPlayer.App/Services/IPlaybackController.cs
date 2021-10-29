using System;
using AudioBookPlayer.App.Core;

namespace AudioBookPlayer.App.Services
{
    public interface IPlaybackController
    {
        /// <summary>
        /// 
        /// </summary>
        long PlaybackPosition { get; }

        /// <summary>
        /// 
        /// </summary>
        long PlaybackDuration { get; }

        /// <summary>
        /// 
        /// </summary>
        PlaybackState PlaybackState { get; }

        /// <summary>
        /// 
        /// </summary>
        event EventHandler PlaybackStateChanged;

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
        void SkipToPrevious();

        /// <summary>
        /// 
        /// </summary>
        void SkipToNext();

        /// <summary>
        /// 
        /// </summary>
        void FastForward();

        /// <summary>
        /// 
        /// </summary>
        void Rewind();
    }
}