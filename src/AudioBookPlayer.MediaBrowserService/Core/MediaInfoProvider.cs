using System.IO;
using LibraProgramming.Media.Common;

namespace AudioBookPlayer.MediaBrowserService.Core
{
    internal abstract class MediaInfoProvider
    {
        protected MediaInfoProvider()
        {

        }

        public abstract MediaInfo ExtractMediaInfo(Stream stream);
    }
}