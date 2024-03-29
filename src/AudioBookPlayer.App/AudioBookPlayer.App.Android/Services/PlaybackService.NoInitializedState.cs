﻿using System;
using AudioBookPlayer.App.Domain.Models;

namespace AudioBookPlayer.App.Android.Services
{
    public sealed partial class PlaybackService
    {
        /// <summary>
        /// 
        /// </summary>
        private sealed class NoInitializedState : PlayerState
        {
            public NoInitializedState(PlaybackService service)
                : base(service)
            {
            }

            public override void SetAudioBook(AudioBook audioBook)
            {
                if (Service.TrySetAudioBook(audioBook))
                {
                    if (0 == audioBook.Chapters.Count)
                    {
                        Service.ResetChapterIndexInternal();
                    }
                    else
                    {
                        Service.SetChapterIndexInternal(0);
                    }

                    Service.CurrentPosition = TimeSpan.Zero;
                }
            }

            public override void SetChapterIndex(int chapterIndex)
            {
                if (Service.SetChapterIndexInternal(chapterIndex))
                {
                    /*var sourceChanged = Service.HasSourceUriChanged();

                    if (sourceChanged && Service.OpenDataSource())
                    {
                        ;
                    }*/
                }
            }

            public override void Play()
            {
                Service.InitializePlayer();
                Service.SetDataSource();

                if (Service.TryStartPlaying(true))
                {
                    Service.State = new PlayingState(Service);
                    return;
                }

                Service.State = new FailedState(Service);
            }

            public override void Pause()
            {
                throw new NotImplementedException();
            }
        }
    }
}