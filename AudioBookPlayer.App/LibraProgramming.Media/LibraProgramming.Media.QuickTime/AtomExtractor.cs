using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace LibraProgramming.Media.QuickTime
{
    internal sealed class AtomExtractor : IEnumerable<Atom>
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

        public IEnumerator<Atom> GetEnumerator()
        {
            return new AtomEnumerator(stream);
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        /// <summary>
        /// 
        /// </summary>
        private class AtomEnumerator : IEnumerator<Atom>
        {
            private readonly Stream stream;

            public Atom Current
            {
                get;
                private set;
            }

            object IEnumerator.Current => Current;

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

            public bool MoveNext()
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

                SeekNextChunk(offset);

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
                ;
            }

            private void SeekNextChunk(long offset)
            {
                Current = ReadChunkFrom(offset);
            }

            private long GetNextChunkOffset() => null != Current ? Current.Stream.Start + Current.Stream.Length : 0L;

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
        }
    }
}