using LibraProgramming.Media.QuickTime.Extensions;
using System;
using System.IO;
using System.Text;

namespace LibraProgramming.Media.QuickTime
{
    public abstract class Chunk
    {
        protected static readonly DateTime QuickTimeEpoch;

        public uint Type
        {
            get;
        }

        protected Chunk(uint type)
        {
            Type = type;
        }

        static Chunk()
        {
            QuickTimeEpoch = new DateTime(1904, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        }

        protected static (byte version, uint flags) ReadFlagsAndVersion(Stream stream)
        {
            var bits = StreamHelper.ReadUInt32(stream);
            var version = (byte)((bits & 0xFF00_0000) >> 24);
            var flags = bits & 0x00FF_FFFF;

            return (version, flags);
        }

        public virtual void Debug(int level)
        {
            var tabs = new String(' ', level);
            var bytes = BitConverter.GetBytes(Type);
            var type = Encoding.ASCII.GetString(bytes.ToBigEndian());

            Console.WriteLine($"{tabs}{type}");
        }

        protected static DateTime ReadUtcDateTime(Stream stream, byte version)
        {
            switch (version)
            {
                case 0:
                {
                    var seconds = StreamHelper.ReadUInt32(stream);
                    return QuickTimeEpoch + TimeSpan.FromSeconds(seconds);
                }

                case 1:
                {
                    var seconds = StreamHelper.ReadInt64(stream);
                    return QuickTimeEpoch + TimeSpan.FromSeconds(seconds);
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