using LibraProgramming.Media.QuickTime.Extensions;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

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

        protected static async Task<(byte version, uint flags)> ReadFlagsAndVersionAsync(Stream stream)
        {
            var bits = await StreamHelper.ReadUInt32Async(stream);
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

        protected static async Task<DateTime> ReadUtcDateTimeAsync(Stream stream, byte version)
        {
            switch (version)
            {
                case 0:
                {
                    var seconds = await StreamHelper.ReadUInt32Async(stream);
                    return QuickTimeEpoch + TimeSpan.FromSeconds(seconds);
                }

                case 1:
                {
                    var seconds = await StreamHelper.ReadInt64Async(stream);
                    return QuickTimeEpoch + TimeSpan.FromSeconds(seconds);
                }

                default:
                {
                    throw new NotSupportedException();
                }
            }
        }

        protected static async Task<ulong> ReadLengthAsync(Stream stream, byte version)
        {
            switch (version)
            {
                case 0:
                {
                    return (ulong)await StreamHelper.ReadUInt32Async(stream);
                }

                case 1:
                {
                    return await StreamHelper.ReadUInt64Async(stream);
                }

                default:
                {
                    throw new NotSupportedException();
                }
            }
        }
    }
}