using LibraProgramming.Media.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace LibraProgramming.Media.QuickTime.Visitors
{
    internal sealed class MediaTrackVisitor : QuickTimeMediaVisitor
    {
        private readonly IList<MediaTrack> tracks;

        public MediaTrackVisitor(IList<MediaTrack> tracks)
        {
            this.tracks = tracks;
        }
    }
}
