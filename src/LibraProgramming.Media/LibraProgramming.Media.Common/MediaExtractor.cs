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

        public abstract IReadOnlyCollection<MetaInformationItem> GetMeta();

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
