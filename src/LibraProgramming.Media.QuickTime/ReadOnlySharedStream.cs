using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace LibraProgramming.Media.QuickTime
{
    internal sealed class ReadOnlySharedStream : Stream
    {
        private readonly Stream stream;
        private long position;
        private long length;

        public ReadOnlySharedStream(Stream stream)
        {
            this.stream = stream;

            position = stream.Position;
            length = stream.Length;
        }

        public override bool CanRead { get; } = true;

        public override bool CanSeek { get; } = true;

        public override bool CanWrite { get; } = false;

        public override long Length => length;

        public override long Position
        {
            get => position;
            set => Seek(value, SeekOrigin.Begin);
        }

        public override void Flush()
        {
            throw new NotImplementedException();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            var value = stream.Read(buffer, offset, count);
            
            position += value;

            return value;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            var value = CalculatePosition(offset, origin);

            if (value != position)
            {
                position = stream.Seek(offset, origin);
            }

            return position;
        }

        public override void SetLength(long value)
        {
            throw new NotImplementedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotImplementedException();
        }

        private long CalculatePosition(long offset, SeekOrigin origin)
        {
            switch (origin)
            {
                case SeekOrigin.Begin:
                {
                    return offset;
                }

                case SeekOrigin.Current:
                {
                    return position + offset;
                }

                case SeekOrigin.End:
                {
                    return length - offset;
                }
            }

            throw new Exception();
        }
    }
}
