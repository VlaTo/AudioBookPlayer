using System;
using System.Threading;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using AudioBookPlayer.App.Models;
using AudioBookPlayer.App.Services;

namespace AudioBookPlayer.App.Android.Services
{
    internal sealed class PlaybackController : IPlaybackController
    {
        private readonly Context context;
        private readonly IPlaybackService service;

        public PlaybackController(IPlaybackService service)
        {
            this.service = service;
            context = Application.Context;
            // var activityManager = (ActivityManager) Application.Context.GetSystemService(Application.ActivityService);
        }
        //var databasePath = Application.Context.GetDatabasePath("temp.db");

        public Task<IPlayback> CreatePlaybackAsync(AudioBook audioBook, CancellationToken cancellationToken = default)
        {
            service.SetBook(audioBook);
            return Task.FromResult<IPlayback>(new Playback(this));
        }

        private void Play()
        {
            ;
        }

        /// <summary>
        /// 
        /// </summary>
        private sealed class Playback : IPlayback
        {
            private PlaybackController controller;
            private bool disposed;

            public Playback(PlaybackController controller)
            {
                this.controller = controller;
            }

            public void SelectChapter(int chapterIndex)
            {
                ;
            }

            public void Play(TimeSpan position)
            {
                controller.Play();
            }

            public void Dispose()
            {
                if (disposed)
                {
                    return;
                }

                Dispose(true);
            }

            private void Dispose(bool dispose)
            {
                if (disposed)
                {
                    return;
                }

                try
                {
                    if (dispose)
                    {
                        controller = null;
                    }
                }
                finally
                {
                    disposed = true;
                }
            }
        }
    }
}