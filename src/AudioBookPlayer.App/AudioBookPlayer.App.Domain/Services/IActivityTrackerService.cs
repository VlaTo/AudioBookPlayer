using System.Threading.Tasks;
using AudioBookPlayer.App.Domain.Models;

namespace AudioBookPlayer.App.Domain.Services
{
    public enum ActivityType
    {
        Play,
        Pause
    }

    public interface IActivityTrackerService
    {
        Task TrackActivityAsync(ActivityType activityType, AudioBookPosition position);
    }
}