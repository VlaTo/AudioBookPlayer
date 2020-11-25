using LibraProgramming.Media.Common;
using System;
using System.IO;

namespace LibraProgramming.Media.QuickTime
{
    public sealed class QuickTimeMediaTrack : MediaTrack
    {
        private readonly QuickTimeMediaExtractor extractor;

        public bool HasReference
        {
            get;
            internal set;
        }

        internal QuickTimeMediaTrack(QuickTimeMediaExtractor extractor)
        {
            this.extractor = extractor;
        }

        public override Stream GetSampleStream()
        {
            return null;
        }

        internal void SetId(int value)
        {
            Id = value;
        }

        internal void SetDuration(TimeSpan value)
        {
            Duration = value;
        }
    }
}
