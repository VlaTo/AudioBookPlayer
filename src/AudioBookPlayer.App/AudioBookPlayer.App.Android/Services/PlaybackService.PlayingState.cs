using System;
using AudioBookPlayer.App.Domain.Models;

namespace AudioBookPlayer.App.Android.Services
{
    public sealed partial class PlaybackService
    {
        /// <summary>
        /// 
        /// </summary>
        private sealed class PlayingState : PlayerState
        {
            public PlayingState(PlaybackService service)
                : base(service)
            {
            }

            public override void SetAudioBook(AudioBook audioBook)
            {
                if (Service.TrySetAudioBook(audioBook))
                {
                    Service.StopPlaying();

                    if (0 == audioBook.Chapters.Count)
                    {
                        Service.ResetChapterIndexInternal();
                    }
                    else
                    {
                        Service.SetChapterIndexInternal(0);
                    }

                    Service.CurrentPosition = TimeSpan.Zero;

                    if (false == Service.TryStartPlaying(true))
                    {
                        Service.State = new FailedState(Service);
                    }
                }
            }

            public override void SetChapterIndex(int chapterIndex)
            {
                if (Service.SetChapterIndexInternal(chapterIndex))
                {
                    var sourceChanged = Service.HasSourceUriChanged();

                    if (sourceChanged)
                    {
                        Service.StopPlaying();
                        Service.SetDataSource();

                        /*Service.CurrentPosition = TimeSpan.Zero;

                        if (Service.TryStartPlaying(true))
                        {

                        }*/
                    }
                    else
                    {
                        Service.PausePlaying();

                        /*Service.CurrentPosition = TimeSpan.Zero;

                        if (Service.TryStartPlaying(true))
                        {

                        }*/
                    }

                    Service.CurrentPosition = TimeSpan.Zero;

                    if (false == Service.TryStartPlaying(true))
                    {
                        Service.State = new FailedState(Service);
                    }
                }
            }

            public override void Play()
            {
                throw new NotImplementedException();
            }

            public override void Pause()
            {
                Service.PausePlaying();
                Service.State = new PausedState(Service);
            }
        }
    }
}