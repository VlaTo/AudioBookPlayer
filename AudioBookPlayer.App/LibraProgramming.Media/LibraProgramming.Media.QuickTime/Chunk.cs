using System;
using System.IO;

namespace LibraProgramming.Media.QuickTime
{
    public abstract class Chunk
    {
        public uint Type
        {
            get;
        }

        protected Chunk(uint type)
        {
            Type = type;
        }

        public abstract void Debug(int level);

        protected static (byte version, uint flags) ReadFlagAndVersion(Stream stream)
        {
            var bits = StreamHelper.ReadUInt32(stream);
            var version = (byte)((bits & 0xFF00_0000) >> 24);
            var flags = bits & 0x00FF_FFFF;

            return (version, flags);
        }

        protected static DateTime ReadUtcDateTime(Stream stream, byte version)
        {
            switch (version)
            {
                case 0:
                {
                    var seconds = StreamHelper.ReadUInt32(stream);
                    return DateTimeOffset.FromUnixTimeSeconds(seconds).UtcDateTime;
                }

                case 1:
                {
                    var milliseconds = StreamHelper.ReadInt64(stream);
                    return DateTimeOffset.FromUnixTimeSeconds(milliseconds).UtcDateTime;
                }

                default:
                {
                    throw new NotSupportedException();
                }
            }
        }

        protected static ulong ReadLength(Stream stream, byte version)
        {
            switch (version)
            {
                case 0:
                {
                    return (ulong)StreamHelper.ReadUInt32(stream);
                }

                case 1:
                {
                    return StreamHelper.ReadUInt64(stream);
                }

                default:
                {
                    throw new NotSupportedException();
                }
            }
        }
    }
}