using System;
using System.IO;

namespace LibraProgramming.Media.QuickTime
{
    public sealed class AtomHeader
    {
        public int Length
        {
            get;
        }

        public long Offset
        {
            get;
        }

        public uint Type
        {
            get;
        }

        public long ChunkLength
        {
            get;
        }

        private AtomHeader(uint type, int length, long offset, long chunkLength)
        {
            Type = type;
            Offset = offset;
            Length = length;
            ChunkLength = chunkLength;
        }

        public static AtomHeader ReadFrom(Stream stream, long offset)
        {
            if (null == stream)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            if (false == stream.CanRead)
            {
                throw new InvalidOperationException();
            }

            var (type, size, length) = StreamHelper.ReadChunkPrefix(stream);

            if (AtomTypes.Empty == type)
            {
                return null;
            }

            return new AtomHeader(type, size, offset, length);
        }
    }
}