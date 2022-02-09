using LibraProgramming.Media.QuickTime.Extensions;
using System;
using System.IO;
using System.Text;

namespace LibraProgramming.Media.QuickTime
{
    internal static class StreamHelper
    {
        // should be same as extended length field size
        public const int PrefixSize = 8;
        
        public static ushort ReadUInt16(Stream stream)
        {
            var data = new byte[sizeof(UInt16)];

            ReadBytesInternal(stream, data, 0, sizeof(UInt16), true);

            return BitConverter.ToUInt16(data, 0);
        }

        public static int ReadInt32(Stream stream)
        {
            var data = new byte[sizeof(Int32)];

            ReadBytesInternal(stream, data, 0, sizeof(Int32), true);

            return BitConverter.ToInt32(data, 0);
        }

        public static long ReadInt64(Stream stream)
        {
            var data = new byte[sizeof(Int64)];

            ReadBytesInternal(stream, data, 0, sizeof(Int64), true);

            return BitConverter.ToInt64(data, 0);
        }

        public static uint ReadUInt32(Stream stream)
        {
            var data = new byte[sizeof(UInt32)];

            ReadBytesInternal(stream, data, 0, sizeof(UInt32), true);

            return BitConverter.ToUInt32(data, 0);
        }

        public static ulong ReadUInt64(Stream stream)
        {
            var data = new byte[sizeof(UInt64)];

            ReadBytesInternal(stream, data, 0, sizeof(UInt64), true);

            return BitConverter.ToUInt64(data, 0);
        }

        public static float ReadFixedPoint(Stream stream, int count)
        {
            if (2 == count)
            {
                var value = ReadUInt16(stream);
                int counter = value >> 12;
                float result = value & 0xfff;

                while (counter > 0)
                {
                    result /= 10.0f;
                    counter--;
                }

                return result;
            }

            if (4 == count)
            {
                var value = ReadUInt32(stream);
                long counter = value >> 12;
                float result = value & 0xfff;

                while (counter > 0)
                {
                    result /= 10.0f;
                    counter--;
                }

                return result;
            }

            throw new NotSupportedException();
        }

        public static string ReadASCIIString(Stream stream, int length)
        {
            var data = new byte[length];
            var count = stream.Read(data, 0, data.Length);

            return Encoding.ASCII.GetString(data, 0, count);
        }

        public static string ReadUnicodeString(Stream stream)
        {
            var length = ReadUInt16(stream);

            if (0 == length)
            {
                return String.Empty;
            }

            var bytes = ReadBytes(stream, length);
            var encoding = Encoding.Unicode;
            var preamble = encoding.GetPreamble();

            if (bytes.StartsWith(preamble))
            {
                return encoding.GetString(bytes, preamble.Length, length - preamble.Length);
            }

            return Encoding.UTF8.GetString(bytes);
        }

        public static byte[] ReadBytes(Stream stream, uint length)
        {
            var data = new byte[length];

            ReadBytesInternal(stream, data, 0, data.Length);

            return data;
        }

        public static byte ReadByte(Stream stream)
        {
            var data = new byte[1];

            ReadBytesInternal(stream, data, 0, 1);

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
            
            ReadBytesInternal(stream, data, 0, sizeof(UInt32), true);
            ReadBytesInternal(stream, data, sizeof(UInt32), sizeof(UInt32), true);

            var length = (long) BitConverter.ToUInt32(data, 0);
            var atomType = BitConverter.ToUInt32(data, sizeof(UInt32));

            if (1U == length)
            {
                size += PrefixSize;

                ReadBytesInternal(stream, data, 0, sizeof(UInt64), true);

                length = BitConverter.ToInt64(data, 0);
            }

            return (atomType, size, length);
        }

        public static void ToLittleEndian(byte[] buffer, int offset, int count)
        {
            if (false == BitConverter.IsLittleEndian)
            {
                return;
            }

            var iterations = count >> 1;
            var start = offset;
            var end = offset + count - 1;

            for (var index = 0; index < iterations; index++)
            {
                var temp = buffer[start];

                buffer[start++] = buffer[end];
                buffer[end--] = temp;
            }
        }

        private static void ReadBytesInternal(Stream stream, byte[] buffer, int offset, int count)
        {
            var actualCount = stream.Read(buffer, offset, count);

            if (actualCount != count)
            {
                throw new InvalidOperationException();
            }
        }

        private static void ReadBytesInternal(Stream stream, byte[] buffer, int offset, int count, bool forceEndian)
        {
            ReadBytesInternal(stream, buffer, offset, count);

            if (forceEndian)
            {
                ToLittleEndian(buffer, offset, count);
            }

            /*if (forceEndian && BitConverter.IsLittleEndian)
            {
                var iterations = count >> 1;
                int start = offset;
                int end = offset + count - 1;

                for (var index = 0; index < iterations; index++)
                {
                    var temp = buffer[start];

                    buffer[start++] = buffer[end];
                    buffer[end--] = temp;
                }
            }*/
        }
    }
}