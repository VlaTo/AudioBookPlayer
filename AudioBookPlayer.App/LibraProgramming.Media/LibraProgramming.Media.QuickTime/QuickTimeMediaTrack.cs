using LibraProgramming.Media.Common;
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
    }
}
