using LibraProgramming.Media.QuickTime.Extensions;
using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace LibraProgramming.Media.QuickTime
{
    internal static class StreamHelper
    {
        // should be same as extended length field size
        public const int PrefixSize = 8;

        public static UInt16 ReadUInt16(Stream stream)
        {
            var data = new byte[sizeof(UInt16)];
            var actualCount = ReadBytesFromStreamInternal(stream, data);

            if (BitConverter.IsLittleEndian)
            {
                var buffer = new byte[data.Length];

                for (var index = 0; index < data.Length; index++)
                {
                    buffer[index] = data[data.Length - index - 1];
                }

                data = buffer;
            }

            return BitConverter.ToUInt16(data, 0);
        }

        public static int ReadInt32(Stream stream)
        {
            var data = new byte[sizeof(int)];
            var actualCount = ReadBytesFromStreamInternal(stream, data);

            if (BitConverter.IsLittleEndian)
            {
                var buffer = new byte[data.Length];

                for (var index = 0; index < data.Length; index++)
                {
                    buffer[index] = data[data.Length - index - 1];
                }

                data = buffer;
            }

            return BitConverter.ToInt32(data, 0);
        }

        public static long ReadInt64(Stream stream)
        {
            var data = new byte[sizeof(long)];
            var actualCount = ReadBytesFromStreamInternal(stream, data);

            if (BitConverter.IsLittleEndian)
            {
                var buffer = new byte[data.Length];

                for (var index = 0; index < data.Length; index++)
                {
                    buffer[index] = data[data.Length - index - 1];
                }

                data = buffer;
            }

            return BitConverter.ToInt64(data, 0);
        }

        public static uint ReadUInt32(Stream stream)
        {
            var data = new byte[sizeof(uint)];
            var actualCount = ReadBytesFromStreamInternal(stream, data);

            if (BitConverter.IsLittleEndian)
            {
                var buffer = new byte[data.Length];

                for (var index = 0; index < data.Length; index++)
                {
                    buffer[index] = data[data.Length - index - 1];
                }

                data = buffer;
            }

            return BitConverter.ToUInt32(data, 0);
        }

        public static ulong ReadUInt64(Stream stream)
        {
            var data = new byte[sizeof(ulong)];
            var actualCount = ReadBytesFromStreamInternal(stream, data);

            if (BitConverter.IsLittleEndian)
            {
                var buffer = new byte[data.Length];

                for (var index = 0; index < data.Length; index++)
                {
                    buffer[index] = data[data.Length - index - 1];
                }

                data = buffer;
            }

            return BitConverter.ToUInt64(data, 0);
        }

        public static uint ReadFlags32(Stream stream)
        {
            var data = new byte[sizeof(uint)];
            var actualCount = ReadBytesFromStreamInternal(stream, data);

            return BitConverter.ToUInt32(data, 0);
        }

        public static string ReadString(Stream stream, int length)
        {
            var data = new byte[length];
            var count = stream.Read(data, 0, data.Length);

            return Encoding.ASCII.GetString(data, 0, count);
        }

        public static string ReadPascalString(Stream stream)
        {
            var length = ReadUInt16(stream);

            if (0 == length)
            {
                return String.Empty;
            }

            var bytes = ReadBytes(stream, length);
            var text = Encoding.Unicode.GetString(bytes);

            return text;
        }

        public static byte[] ReadBytes(Stream stream, uint length)
        {
            var data = new byte[length];
            var actualCount = ReadBytesFromStreamInternal(stream, data);

            return data;
        }

        public static byte ReadByte(Stream stream)
        {
            var data = new byte[1];
            var actualCount = ReadBytesFromStreamInternal(stream, data);

            return data[0];
        }

        public static (uint type, int size, long length) ReadChunkPrefix(Stream stream)
        {
            var size = PrefixSize;
            var available = stream.Length - stream.Position;

            if (PrefixSize > available)
            {
                return (AtomTypes.Empty, 0, -1);
            }

            var data = new byte[size];
            var count = ReadBytesFromStreamInternal(stream, data);

            var length = (long)BitConverter.ToUInt32(data.Slice(0, 4).ToBigEndian(), 0);
            var atomType = BitConverter.ToUInt32(data.Slice(4).ToBigEndian(), 0);

            if (1U == length)
            {
                size += PrefixSize;

                var extra = ReadBytesFromStreamInternal(stream, data);

                length = BitConverter.ToInt64(data.ToBigEndian(), 0);
            }

            return (atomType, size, length);
        }

        private static int ReadBytesFromStreamInternal(Stream stream, byte[] bytes)
        {
            var bufferLength = bytes.Length;
            var actualCount = stream.Read(bytes, 0, bufferLength);

            if (bufferLength != actualCount)
            {
                throw new InvalidOperationException();
            }

            return actualCount;
        }
    }
}