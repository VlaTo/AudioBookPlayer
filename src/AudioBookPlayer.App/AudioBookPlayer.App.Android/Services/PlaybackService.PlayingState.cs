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
                if (Service.SetAudioBookInternal(audioBook))
                {
                    Service.StopPlayingInternal();

                    if (0 == audioBook.Chapters.Count)
                    {
                        Service.ResetChapterIndexInternal();
                    }
                    else
                    {
                        Service.SetChapterIndexInternal(0);
                    }

                    Service.CurrentPosition = TimeSpan.Zero;
                    Service.StartPlayingInternal(true);
                }
            }

            public override void SetChapterIndex(int chapterIndex)
            {
                if (Service.SetChapterIndexInternal(chapterIndex))
                {
                    var sourceChanged = Service.HasSourceUriChanged();

                    if (sourceChanged)
                    {
                        Service.StopPlayingInternal();

                        if (Service.OpenDataSource())
                        {
                            Service.CurrentPosition = TimeSpan.Zero;
                            Service.StartPlayingInternal(true);
                        }
                    }
                    else
                    {
                        Service.PausePlayingInternal();
                        Service.CurrentPosition = TimeSpan.Zero;
                        Service.StartPlayingInternal(true);
                    }
                }
            }

            public override void Play()
            {
                throw new NotImplementedException();
            }

            public override void Pause()
            {
                Service.PausePlayingInternal();
                Service.State = new PausedState(Service);
            }
        }
    }
}