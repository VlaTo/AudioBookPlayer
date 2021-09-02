using System.Diagnostics;
using System.Threading.Tasks;
using AudioBookPlayer.App.Domain.Models;
using AudioBookPlayer.App.Domain.Services;

namespace AudioBookPlayer.App.Services
{
    public sealed class ActivityTrackerService : IActivityTrackerService
    {
        public Task TrackActivityAsync(ActivityType activityType, AudioBookPosition position)
        {
            Debug.WriteLine($"Activity: {activityType} for (book: {position.BookId}, chapter: {position.ChapterIndex}, offset: {position.ChapterPosition:g})");
            return Task.CompletedTask;
        }
    }
}