using System;
using System.Collections.Generic;

namespace LibraProgramming.Media.Common
{
    public abstract class MediaExtractor : IDisposable
    {
        private bool disposed;

        protected MediaExtractor()
        {
        }

        public abstract IReadOnlyCollection<IMediaTrack> GetTracks();

        public abstract MediaTags GetMediaTags();

        public void Dispose() => Dispose(true);

        protected void EnsureNotDisposed()
        {
            if (disposed)
            {
                throw new ObjectDisposedException(typeof(MediaExtractor).Name);
            }
        }

        protected virtual void DoDispose()
        {
        }

        private void Dispose(bool dispose)
        {
            if (disposed)
            {
                return;
            }

            try
            {
                if (dispose)
                {
                    DoDispose();
                }
            }
            finally
            {
                disposed = true;
            }
        }
    }
}
