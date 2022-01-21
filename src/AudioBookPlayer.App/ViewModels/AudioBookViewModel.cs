using System;

namespace AudioBookPlayer.App.ViewModels
{
    internal sealed class AudioBookViewModel : Java.Lang.Object
    {
        public string Title
        {
            get;
        }

        public string Subtitle
        {
            get;
        }

        public string Description
        {
            get;
        }

        public TimeSpan Duration
        {
            get;
        }

        public string Authors
        {
            get;
        }

        public string MediaId
        {
            get;
        }

        public AudioBookViewModel(
            string mediaId,
            string title,
            string subtitle,
            string description,
            string authors,
            TimeSpan duration,
            DateTime created)
        {
            MediaId = mediaId;
            Title = title;
            Subtitle = subtitle;
            Description = description;
            Duration = duration;
            Authors = authors;
        }
    }
}