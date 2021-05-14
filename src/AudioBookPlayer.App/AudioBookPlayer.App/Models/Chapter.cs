using System;

namespace AudioBookPlayer.App.Models
{
    public sealed class Chapter
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
    }
}