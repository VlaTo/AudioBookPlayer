using System;

namespace AudioBookPlayer.App.Models
{
    public sealed class AudioBookChapterFragment
    {
        public TimeSpan Start
        {
            get;
        }

        public TimeSpan Duration
        {
            get;
        }

        public TimeSpan End
        {
            get;
        }

        public AudioBookSourceFile SourceFile
        {
            get;
        }

        public AudioBookChapterFragment(TimeSpan start, TimeSpan duration, AudioBookSourceFile sourceFile)
        {
            Start = start;
            Duration = duration;
            End = start + duration;
            SourceFile = sourceFile;
        }
    }
}