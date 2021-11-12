using System;
using Android.OS;
using AudioBookPlayer.App.Models;

namespace AudioBookPlayer.App.Android.Models
{
    internal sealed class SectionMetadata : ISectionMetadata
    {
        private readonly Bundle bundle;

        public string Name => bundle.GetString("Section.Name");

        public int Index => bundle.GetInt("Section.Index");

        public TimeSpan Start
        {
            get
            {
                var milliseconds = bundle.GetLong("Chapter.Start");
                return TimeSpan.FromMilliseconds(milliseconds);
            }
        }

        public TimeSpan Duration
        {
            get
            {
                var milliseconds = bundle.GetLong("Chapter.Duration");
                return TimeSpan.FromMilliseconds(milliseconds);
            }
        }

        public SectionMetadata(Bundle bundle)
        {
            this.bundle = bundle;
        }
    }
}