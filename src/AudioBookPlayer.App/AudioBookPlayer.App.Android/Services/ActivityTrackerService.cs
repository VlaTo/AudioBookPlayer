using System;
using System.Diagnostics.CodeAnalysis;
using AudioBookPlayer.App.Domain.Models;
using AudioBookPlayer.App.Domain.Services;
using AudioBookPlayer.App.Persistence.LiteDb;
using AudioBookPlayer.App.Persistence.LiteDb.Models;

namespace AudioBookPlayer.App.Android.Services
{
    internal sealed class ActivityTrackerService : IActivityTrackerService
    {
        private readonly LiteDbContext context;

        public ActivityTrackerService(LiteDbContext context)
        {
            this.context = context;
        }

        public void TrackActivity(ActivityType activityType, AudioBookPosition position)
        {
            using (var unitOfWork = new UnitOfWork(context, false))
            {
                var activity = CreateActivity(position);

                unitOfWork.Activities.Add(activity);
                unitOfWork.Commit();
            }
        }

        [return: MaybeNull]
        public AudioBookPosition GetRecent()
        {
            using (var unitOfWork = new UnitOfWork(context, false))
            {
                var activity = unitOfWork.Activities.GetRecent();

                if (null != activity)
                {
                    return CreateAudioBookPosition(activity);
                }
            }

            return null;
        }

        private static Activity CreateActivity(AudioBookPosition position)
        {
            var instance = new Activity
            {
                Id = (long)position.MediaId.BookId,
                MediaId = position.MediaId.ToString(),
                QueueId = position.QueueItemId,
                MediaPosition = position.MediaPosition,
                Time = DateTime.UtcNow
            };

            return instance;
        }
        
        private static AudioBookPosition CreateAudioBookPosition(Activity activity)
        {
            var mediaId = MediaId.Parse(activity.MediaId);
            var instance = new AudioBookPosition(mediaId, activity.QueueId, activity.MediaPosition);
            return instance;
        }
    }
}