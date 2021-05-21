using System;

namespace AudioBookPlayer.App.Models
{
    public sealed class AudioBookChapter
    {
        public string Title
        {
            get; 
            set;
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

        public string SourceFile
        {
            get;
            set;
        }
    }
}