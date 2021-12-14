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

        public string MediaId
        {
            get;
        }

        public AudioBookDescription(string mediaId, string title, TimeSpan duration)
        {
            MediaId = mediaId;
            Title = title;
            Duration = duration;
        }
    }
}