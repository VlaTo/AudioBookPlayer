using System;
using System.IO;
using System.Threading.Tasks;

namespace LibraProgramming.Media.QuickTime
{
    internal sealed class AtomExtractor
    {
        private readonly Stream stream;

        public AtomExtractor(Stream stream)
        {
            if (null == stream)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            this.stream = stream;
        }

        public AtomEnumerator GetEnumerator()
        {
            return new AtomEnumerator(stream);
        }

        /// <summary>
        /// 
        /// </summary>
        internal class AtomEnumerator : IDisposable
        {
            private Stream stream;
            private bool disposed;

            public Atom Current
            {
                get;
                private set;
            }

            public bool IsEOF
            {
                get;
                private set;
            }

            public AtomEnumerator(Stream stream)
            {
                this.stream = stream;

                Current = null;
                IsEOF = false;
            }

            public async Task<bool> MoveNextAsync()
            {
                if (IsEOF)
                {
                    return false;
                }

                var offset = GetNextChunkOffset();

                if (0L > offset)
                {
                    return false;
                }

                await SeekNextChunkAsync(offset);

                return null != Current;
            }

            public void Reset()
            {
                var offset = stream.Seek(0L, SeekOrigin.Begin);

                if (0L != offset)
                {
                    throw new Exception();
                }

                Current = null;
                IsEOF = false;
            }

            public void Dispose()
            {
                if (disposed)
                {
                    return;
                }

                Dispose(true);
            }

            private async Task SeekNextChunkAsync(long offset)
            {
                Current = await ReadChunkAsync(offset);
            }

            private long GetNextChunkOffset()
            {
                if (null != Current)
                {
                    return Current.Stream.Start + Current.Stream.Length;
                }

                return 0L;
            }

            private async Task<Atom> ReadChunkAsync(long offset)
            {
                try
                {
                    var position = stream.Seek(offset, SeekOrigin.Begin);

                    if (position != offset)
                    {
                        IsEOF = true;
                        return null;
                    }

                    var header = await AtomHeader.ReadFromAsync(stream, offset);

                    if (null != header)
                    {
                        return new Atom(header, stream);
                    }
                }
                catch (Exception exception)
                {
                    IsEOF = true;
                }

                return null;
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
                        stream.Dispose();
                        stream = null;
                    }
                }
                finally
                {
                    disposed = true;
                }
            }
        }
    }
}