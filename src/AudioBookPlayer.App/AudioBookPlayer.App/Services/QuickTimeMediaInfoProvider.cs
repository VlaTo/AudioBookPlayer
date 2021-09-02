using LibraProgramming.Media.Common;
using LibraProgramming.Media.QuickTime;
using System.IO;

namespace AudioBookPlayer.App.Services
{
    internal sealed class QuickTimeMediaInfoProvider : IMediaInfoProvider
    {
        private readonly string mimeType;

        public QuickTimeMediaInfoProvider(string  mimeType)
        {
            this.mimeType = mimeType;
        }

        public MediaInfo ExtractMediaInfo(Stream stream)
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