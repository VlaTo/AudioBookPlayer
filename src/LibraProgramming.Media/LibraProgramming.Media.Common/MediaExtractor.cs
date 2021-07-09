using System;
using System.Collections.Generic;
using System.Diagnostics;

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

        public void Dispose()
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            Dispose(true);

            stopwatch.Stop();
            Debug.WriteLine($"[MediaExtractor] Dispose took: {stopwatch.Elapsed:g}");
        }

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
