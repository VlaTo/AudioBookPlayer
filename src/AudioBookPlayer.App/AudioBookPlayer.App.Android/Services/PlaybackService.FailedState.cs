using System;
using AudioBookPlayer.App.Domain.Models;

namespace AudioBookPlayer.App.Android.Services
{
    /// <summary>
    /// 
    /// </summary>
    public sealed partial class PlaybackService
    {
        private sealed class FailedState : PlayerState
        {
            public FailedState(PlaybackService service)
                : base(service)
            {
            }

            public override void SetAudioBook(AudioBook audioBook)
            {
                throw new NotImplementedException();
            }

            public override void SetChapterIndex(int chapterIndex)
            {
                throw new NotImplementedException();
            }

            public override void Play()
            {
                throw new NotImplementedException();
            }

            public override void Pause()
            {
                throw new NotImplementedException();
            }
        }
    }
}