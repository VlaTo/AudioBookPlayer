using System;
using System.IO;

namespace LibraProgramming.Media.QuickTime
{
    public sealed class ReadOnlyAtomStream : Stream
    {
        private readonly Stream stream;
        private long position;

        public override bool CanRead { get; } = true;

        public override bool CanSeek { get; } = true;

        public override bool CanWrite { get; } = false;

        public long Start
        {
            get;
        }

        public override long Length
        {
            get;
        }

        public override long Position
        {
            get => position;
            set => Seek(value, SeekOrigin.Begin);
        }

        internal ReadOnlyAtomStream(Stream stream, long start, long length)
        {
            this.stream = stream;
            position = 0L;

            Start = start;
            Length = length;
        }

        public override void Flush()
        {
            throw new NotImplementedException();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            var current = stream.Seek(Start + position, SeekOrigin.Begin);
            var temp = stream.Read(buffer, offset, count);

            position += temp;

            return temp;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            //EnsureOffsetValid(offset, origin);

            switch (origin)
            {
                case SeekOrigin.Begin:
                {
                    if (0L > offset || Length < offset)
                    {
                        throw new ArgumentOutOfRangeException(nameof(offset), offset, "");
                    }

                    position = offset;

                    break;
                }

                case SeekOrigin.Current:
                {
                    var current = position + offset;

                    if (current < Start || current > Length)
                    {
                        throw new ArgumentOutOfRangeException(nameof(offset), offset, "");
                    }

                    position += offset;

                    break;
                }

                case SeekOrigin.End:
                {
                    var current = stream.Position + offset;

                    if (current < Start || current > Length)
                    {
                        throw new ArgumentOutOfRangeException(nameof(offset), offset, "");
                    }

                    position = Length - offset;

                    break;
                }
            }

            return position;
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException();
        }

        private void EnsureOffsetValid(long offset, SeekOrigin origin)
        {
            switch (origin)
            {
                case SeekOrigin.Begin:
                {
                    if (0L > offset || Length < offset)
                    {
                        throw new ArgumentOutOfRangeException(nameof(offset), offset, "");
                    }
                    
                    break;
                }

                case SeekOrigin.Current:
                {
                    var current = position + offset;

                    if (current < Start || current > Length)
                    {
                        throw new ArgumentOutOfRangeException(nameof(offset), offset, "");
                    }

                    break;
                }

                case SeekOrigin.End:
                {
                    var current = stream.Position + offset;

                    if (current < Start || current > Length)
                    {
                        throw new ArgumentOutOfRangeException(nameof(offset), offset, "");
                    }

                    break;
                }
            }
        }
    }
}