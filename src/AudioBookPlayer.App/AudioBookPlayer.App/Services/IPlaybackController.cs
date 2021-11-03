using System;
using AudioBookPlayer.App.Core;

namespace AudioBookPlayer.App.Services
{
    public interface IPlaybackController
    {
        /// <summary>
        /// 
        /// </summary>
        long Position { get; }

        /// <summary>
        /// 
        /// </summary>
        long Offset { get; }

        /// <summary>
        /// 
        /// </summary>
        long Duration { get; }

        /// <summary>
        /// 
        /// </summary>
        PlaybackState State { get; }

        /// <summary>
        /// 
        /// </summary>
        event EventHandler StateChanged;

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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="position"></param>
        void SeekTo(long position);
    }
}