using System.IO;
using LibraProgramming.Media.Common;
using LibraProgramming.Media.QuickTime;

namespace AudioBookPlayer.MediaBrowserService.Core
{
    internal sealed class QuickTimeMediaInfoProvider : MediaInfoProvider
    {
        private readonly string mimeType;

        public QuickTimeMediaInfoProvider(string mimeType)
        {
            this.mimeType = mimeType;
        }

        public override MediaInfo ExtractMediaInfo(Stream stream)
        {
            using (var extractor = QuickTimeMediaExtractor.CreateFrom(stream))
            {
                var mediaTags = extractor.GetMediaTags();
                var mediaTracks = extractor.GetTracks();

                return new MediaInfo(mediaTracks, mediaTags);
            }
        }
    }
}