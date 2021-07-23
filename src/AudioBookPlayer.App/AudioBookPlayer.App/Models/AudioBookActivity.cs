using System;

namespace AudioBookPlayer.App.Models
{
    public sealed class AudioBookActivity
    {
        public BookActivity Activity
        {
            get;
        }

        public TimeSpan When
        {
            get;
        }

        public AudioBookActivity(BookActivity activity, TimeSpan @when)
        {
            Activity = activity;
            When = when;
        }
    }
}