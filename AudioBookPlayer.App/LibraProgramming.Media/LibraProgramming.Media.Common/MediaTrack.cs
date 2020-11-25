using System;
using System.IO;

namespace LibraProgramming.Media.Common
{
    public abstract class MediaTrack
    {
        public int Id
        {
            get;
            protected set;
        }

        public TimeSpan Duration
        {
            get;
            protected set;
        }

        protected MediaTrack()
        {
        }

        public abstract Stream GetSampleStream();
    }
}
