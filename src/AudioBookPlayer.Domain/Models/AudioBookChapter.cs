using System;

namespace AudioBookPlayer.Domain.Models
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
            set;
        }

        public TimeSpan Offset
        {
            get; 
            set;
        }

        public TimeSpan Duration
        {
            get; 
            set;
        }

        public TimeSpan End => Offset + Duration;

        public AudioBookSection Section
        {
            get;
        }

        public AudioBookChapter(AudioBook audioBook, AudioBookSection section)
        {
            AudioBook = audioBook;
            Section = section;
            Offset = TimeSpan.Zero;
            Duration = TimeSpan.Zero;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Offset.GetHashCode();

                hashCode = (hashCode * 397) ^ (Title?.GetHashCode() ?? 0);
                hashCode = (hashCode * 397) ^ Duration.GetHashCode();

                return hashCode;
            }
        }
    }
}