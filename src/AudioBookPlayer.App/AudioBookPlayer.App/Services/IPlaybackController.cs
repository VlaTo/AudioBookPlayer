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

        int ChapterIndex
        {
            get;
            set;
        }

        TimeSpan Position
        {
            get;
        }

        bool IsPlaying
        {
            get;
        }

        event EventHandler IsPlayingChanged;

        event EventHandler AudioBookChanged;

        event EventHandler ChapterIndexChanged;

        event EventHandler PositionChanged;

        void Play(TimeSpan position);

        void Stop();
    }
}