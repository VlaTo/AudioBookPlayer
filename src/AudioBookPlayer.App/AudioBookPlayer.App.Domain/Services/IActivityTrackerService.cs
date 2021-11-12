using AudioBookPlayer.App.Domain.Models;

namespace AudioBookPlayer.App.Domain.Services
{
    public enum ActivityType
    {
        Play,
        Pause,
        Stop
    }

    public interface IActivityTrackerService
    {
        void TrackActivity(ActivityType activityType, AudioBookPosition position);

        AudioBookPosition GetRecent();
    }
}