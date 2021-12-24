using System.Collections.Generic;

namespace LibraProgramming.Media.Common
{
    public sealed class MediaInfo
    {
        public IReadOnlyCollection<IMediaTrack> Tracks
        {
            get;
        }

        public  MediaTags Meta
        {
            get;
        }

        public MediaInfo(IReadOnlyCollection<IMediaTrack> tracks, MediaTags meta)
        {
            Tracks = tracks;
            Meta = meta;
        }
    }
}