using LibraProgramming.Media.Common;
using System;
using System.IO;

namespace LibraProgramming.Media.QuickTime
{
    public sealed class QuickTimeMediaTrack : MediaTrack
    {
        private readonly QuickTimeMediaExtractor extractor;

        internal QuickTimeMediaTrack(QuickTimeMediaExtractor extractor)
        {
            this.extractor = extractor;
        }

        public override Stream GetSampleStream()
        {
            return null;
        }

        internal void SetDuration(TimeSpan value)
        {
            Duration = value;
        }

        internal void SetTitle(string value)
        {
            Title = value;
        }
    }
}
