﻿using System;
using AudioBookPlayer.App.Domain.Models;

namespace AudioBookPlayer.App.Android.Services
{
    public sealed partial class PlaybackService
    {
        private sealed class PausedState : PlayerState
        {
            private bool forceReset;

            public PausedState(PlaybackService service)
                : base(service)
            {
                forceReset = false;
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

                    if (sourceChanged && Service.OpenDataSource())
                    {
                        ;
                    }

                    forceReset = true;
                    Service.CurrentPosition = TimeSpan.Zero;
                }
            }

            public override void Play()
            {
                Service.StartPlayingInternal(forceReset);
                //Service.CurrentPosition = TimeSpan.Zero;
                Service.State = new PlayingState(Service);
            }

            public override void Pause()
            {
                throw new NotImplementedException();
            }
        }
    }
}