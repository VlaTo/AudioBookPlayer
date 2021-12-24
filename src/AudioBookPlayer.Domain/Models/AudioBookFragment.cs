using System;

namespace AudioBookPlayer.Domain.Models
{
    public sealed class AudioBookFragment
    {
        public AudioBook AudioBook
        {
            get;
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

        public AudioBookSourceFile SourceFile
        {
            get;
        }

        public AudioBookFragment(AudioBook audioBook, AudioBookSourceFile sourceFile)
        {
            AudioBook = audioBook;
            Offset = TimeSpan.Zero;
            Duration = TimeSpan.Zero;
            SourceFile = sourceFile;
        }
    }
}