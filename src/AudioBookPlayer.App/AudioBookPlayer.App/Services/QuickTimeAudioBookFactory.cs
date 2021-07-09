using AudioBookPlayer.App.Core;
using LibraProgramming.Media.Common;
using LibraProgramming.Media.QuickTime;
using System.IO;

namespace AudioBookPlayer.App.Services
{
    internal sealed class QuickTimeAudioBookFactory : IAudioBookFactory
    {
        public MediaInfo ExtractMediaInfo(Stream stream)
        {
            using (StopwatchMeter.Start("ExtractMediaInfo call"))
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
}