using System;

namespace AudioBookPlayer.App.Domain.Models
{
    public sealed class AudioBookActivity
    {
        public AudioBookActivityType ActivityType
        {
            get;
        }

        public DateTime DateTime
        {
            get;
        }

        public AudioBookActivity(AudioBookActivityType activityType, DateTime dateTime)
        {
            ActivityType = activityType;
            DateTime = dateTime;
        }
    }
}