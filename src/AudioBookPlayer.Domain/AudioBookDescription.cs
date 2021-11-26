using System;

namespace AudioBookPlayer.Domain
{
    public sealed class AudioBookDescription
    {
        public string Title
        {
            get;
        }

        public TimeSpan Duration
        {
            get;
        }

        public AudioBookDescription(string title, TimeSpan duration)
        {
            Title = title;
        }
    }
}