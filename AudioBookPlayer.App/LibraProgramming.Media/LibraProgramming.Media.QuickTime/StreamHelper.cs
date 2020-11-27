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
        
        public static readonly Encoding Encoding = System.Text.Encoding.Unicode;

        public static UInt16 ReadUInt16(Stream stream)
        {
            if (false == TryReadBytesFromStream(stream, sizeof(UInt16), out var bytes))
            {
                throw new Exception();
            }

            return BitConverter.ToUInt16(bytes, 0);

            /*var data = new byte[sizeof(UInt16)];
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

            return BitConverter.ToUInt16(data, 0);*/
        }

        public static int ReadInt32(Stream stream)
        {
            if (false == TryReadBytesFromStream(stream, sizeof(int), out var bytes))
            {
                throw new Exception();
            }

            /*var data = new byte[sizeof(int)];
            var actualCount = ReadBytesFromStreamInternal(stream, data);

            if (BitConverter.IsLittleEndian)
            {
                var buffer = new byte[data.Length];

                for (var index = 0; index < data.Length; index++)
                {
                    buffer[index] = data[data.Length - index - 1];
                }

                data = buffer;
            }*/

            return BitConverter.ToInt32(bytes, 0);
        }

        public static long ReadInt64(Stream stream)
        {
            if (false == TryReadBytesFromStream(stream, sizeof(long), out var bytes))
            {
                throw new Exception();
            }

            /*var data = new byte[sizeof(long)];
            var actualCount = ReadBytesFromStreamInternal(stream, data);

            if (BitConverter.IsLittleEndian)
            {
                var buffer = new byte[data.Length];

                for (var index = 0; index < data.Length; index++)
                {
                    buffer[index] = data[data.Length - index - 1];
                }

                data = buffer;
            }*/

            return BitConverter.ToInt64(bytes, 0);
        }

        public static uint ReadUInt32(Stream stream)
        {
            if (false == TryReadBytesFromStream(stream, sizeof(uint), out var bytes))
            {
                throw new Exception();
            }

            /*var data = new byte[sizeof(uint)];
            var actualCount = ReadBytesFromStreamInternal(stream, data);

            if (BitConverter.IsLittleEndian)
            {
                var buffer = new byte[data.Length];

                for (var index = 0; index < data.Length; index++)
                {
                    buffer[index] = data[data.Length - index - 1];
                }

                data = buffer;
            }*/

            return BitConverter.ToUInt32(bytes, 0);
        }

        public static ulong ReadUInt64(Stream stream)
        {
            if (false == TryReadBytesFromStream(stream, sizeof(ulong), out var bytes))
            {
                throw new Exception();
            }

            /*var data = new byte[sizeof(ulong)];
            var actualCount = ReadBytesFromStreamInternal(stream, data);

            if (BitConverter.IsLittleEndian)
            {
                var buffer = new byte[data.Length];

                for (var index = 0; index < data.Length; index++)
                {
                    buffer[index] = data[data.Length - index - 1];
                }

                data = buffer;
            }*/

            return BitConverter.ToUInt64(bytes, 0);
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
            var offset = 0;
            var preamble = Encoding.GetPreamble();

            if (true)
            {
                for (var index = 0; index < preamble.Length; index++)
                {
                    if (preamble[index] == bytes[index])
                    {
                        offset = index + 1;
                        length--;
                    }
                }
            }

            return Encoding.GetString(bytes, offset, length);
        }

        public static byte[] ReadBytes(Stream stream, uint length)
        {
            var data = new byte[length];
            var actualCount = ReadBytesFromStream(stream, data, true);

            return data;
        }

        public static byte ReadByte(Stream stream)
        {
            var data = new byte[1];
            var actualCount = ReadBytesFromStream(stream, data, true);

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
            var count = ReadBytesFromStream(stream, data);

            var length = (long)BitConverter.ToUInt32(data.Slice(0, 4).ToBigEndian(), 0);
            var atomType = BitConverter.ToUInt32(data.Slice(4).ToBigEndian(), 0);

            if (1U == length)
            {
                size += PrefixSize;

                var extra = ReadBytesFromStream(stream, data, true);

                length = BitConverter.ToInt64(data.ToBigEndian(), 0);
            }

            return (atomType, size, length);
        }

        private static int ReadBytesFromStream(Stream stream, byte[] bytes, bool throwException = false)
        {
            var bufferLength = bytes.Length;
            var actualCount = stream.Read(bytes, 0, bufferLength);

            if ((bufferLength != actualCount) && throwException)
            {
                throw new InvalidOperationException();
            }

            return actualCount;
        }

        private static bool TryReadBytesFromStream(Stream stream, int count, out byte[] bytes)
        {
            var buffer = new byte[count];
            //var actualCount = stream.Read(buffer, 0, count);
            var actualCount = ReadBytesFromStream(stream, buffer);

            bytes = null;

            if (count != actualCount)
            {
                return false;
            }

            if (BitConverter.IsLittleEndian)
            {
                var iterations = actualCount >> 1;
                int start = 0;
                int end = actualCount - 1;

                for (var index = 0; index < iterations; index++)
                {
                    var place = buffer[start];

                    buffer[start++] = buffer[end];
                    buffer[end--] = place;
                }
            }

            bytes = buffer;

            return true;
        }
    }
}