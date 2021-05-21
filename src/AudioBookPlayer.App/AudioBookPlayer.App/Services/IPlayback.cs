using System;

namespace AudioBookPlayer.App.Services
{
    /// <summary>
    /// 
    /// </summary>
    public interface IPlayback : IDisposable
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="chapterIndex"></param>
        void SelectChapter(int chapterIndex);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="position"></param>
        void Play(TimeSpan position);
    }
}