using System;
using System.Threading;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using AudioBookPlayer.App.Models;
using AudioBookPlayer.App.Services;

namespace AudioBookPlayer.App.Droid.Services
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
            service.Play(chapterIndex, position);

            return Task.FromResult()
        }

        private sealed class Playback : IPlayback
        {
            public Playback()
            {
            }

            public void Dispose()
            {
                throw new NotImplementedException();
            }

            public void SelectChapter(int chapterIndex)
            {
                throw new NotImplementedException();
            }

            public void Play(TimeSpan position)
            {
                throw new NotImplementedException();
            }
        }
    }
}