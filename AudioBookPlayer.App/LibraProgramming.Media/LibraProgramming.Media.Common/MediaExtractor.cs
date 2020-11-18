using System;
using System.Collections.Generic;
using System.IO;

namespace LibraProgramming.Media.Common
{
    public abstract class MediaExtractor : IDisposable
    {
        private bool disposed;

        protected MediaExtractor()
        {
        }

        public abstract int GetTracksCount();

        public abstract MediaTrack GetTrack(int index);

        public abstract MetaInformation GetMeta();

        public void Dispose()
        {
            Dispose(true);
        }

        protected void EnsureNotDisposed()
        {
            if (disposed)
            {
                throw new ObjectDisposedException(typeof(MediaExtractor).Name);
            }
        }

        protected void DoDispose()
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
