using System.IO;

namespace LibraProgramming.Media.Common
{
    public abstract class MediaTrack
    {
        protected MediaTrack()
        {
        }

        public abstract Stream GetSampleStream();
    }
}
