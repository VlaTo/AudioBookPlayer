using System;

namespace AudioBookPlayer.App.Domain.Models
{
    public sealed class AudioBookChapter
    {
        public AudioBook AudioBook
        {
            get;
        }

        public string Title
        {
            get; 
        }

        public TimeSpan Start
        {
            get;
            set;
        }

        public TimeSpan Duration
        {
            get;
            set;
        }

        public TimeSpan End => Start + Duration;

        public AudioBookSection Section
        {
            get;
        }

        public AudioBookChapter(AudioBook audioBook, string title, AudioBookSection section)
        {
            AudioBook = audioBook;
            Title = title;
            Section = section;
        }
    }
}