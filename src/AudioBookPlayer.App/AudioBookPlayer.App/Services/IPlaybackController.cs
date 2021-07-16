using System;
using AudioBookPlayer.App.Models;

namespace AudioBookPlayer.App.Services
{
    /// <summary>
    /// 
    /// </summary>
    public interface IPlaybackController
    {
        AudioBook AudioBook
        {
            get; 
            set;
        }

        event EventHandler IsPlayingChanged;

        event EventHandler AudioBookChanged;

        event EventHandler CurrentChapterChanged;

        event EventHandler CurrentPositionChanged;

        TimeSpan CurrentPosition
        {
            get;
        }

        bool IsPlaying
        {
            get;
        }

        void Play(TimeSpan position);

        void Stop();
    }
}