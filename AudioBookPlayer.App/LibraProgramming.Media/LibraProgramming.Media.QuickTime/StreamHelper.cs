using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace LibraProgramming.Media.QuickTime
{
    internal static class StreamHelper
    {
        // should be same as extended length field size
        public const int PrefixSize = 8;
        
        public static readonly Encoding Encoding = Encoding.Unicode;

        public static async Task<UInt16> ReadUInt16Async(Stream stream)
        {
            var data = new byte[sizeof(UInt16)];

            /*if (false == TryReadBytesFromStream(stream, sizeof(UInt16), out var bytes))
            {
                throw new Exception();
            }*/

            await ReadBytesFromStreamAsync(stream, data, 0, data.Length);

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
            }*/

            return BitConverter.ToUInt16(data, 0);
        }

        public static async Task<int> ReadInt32Async(Stream stream)
        {
            var data = new byte[sizeof(int)];

            /*if (false == TryReadBytesFromStream(stream, sizeof(int), out var bytes))
            {
                throw new Exception();
            }*/

            await ReadBytesFromStreamAsync(stream, data, 0, data.Length);

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

            return BitConverter.ToInt32(data, 0);
        }

        public static async Task<long> ReadInt64Async(Stream stream)
        {
            var data = new byte[sizeof(long)];

            /*if (false == TryReadBytesFromStream(stream, sizeof(long), out var bytes))
            {
                throw new Exception();
            }*/

            await ReadBytesFromStreamAsync(stream, data, 0, data.Length);

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

            return BitConverter.ToInt64(data, 0);
        }

        public static async Task<uint> ReadUInt32Async(Stream stream)
        {
            var data = new byte[sizeof(uint)];

            /*if (false == TryReadBytesFromStream(stream, sizeof(uint), out var bytes))
            {
                throw new Exception();
            }*/

            await ReadBytesFromStreamAsync(stream, data, 0, data.Length);

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

            return BitConverter.ToUInt32(data, 0);
        }

        public static async Task<ulong> ReadUInt64Async(Stream stream)
        {
            var data = new byte[sizeof(ulong)];

            await ReadBytesFromStreamAsync(stream, data, 0, data.Length);

            /*var bytes = await ReadBytesFromStreamAsync(stream, sizeof(ulong), true);
            {
                throw new Exception();
            }*/

            return BitConverter.ToUInt64(data, 0);
        }

        public static async Task<string> ReadStringAsync(Stream stream, int length)
        {
            var data = new byte[length];
            
            await ReadBytesFromStreamAsync(stream, data, 0, length);

            //var count = await stream.ReadAsync(data, 0, data.Length);

            return Encoding.ASCII.GetString(data, 0, length);
        }

        public static async Task<string> ReadPascalStringAsync(Stream stream)
        {
            var length = await ReadUInt16Async(stream);

            if (0 == length)
            {
                return String.Empty;
            }

            var offset = 0;
            var bytes = await ReadBytesAsync(stream, length);
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

        public static async Task<byte[]> ReadBytesAsync(Stream stream, uint length)
        {
            var data = new byte[length];

            //await ReadBytesFromStreamAsync(stream, data, true);
            await ReadBytesFromStreamAsync(stream, data, 0, data.Length);

            return data;
        }

        public static async Task<byte> ReadByteAsync(Stream stream)
        {
            var data = new byte[1];

            //await ReadBytesFromStreamAsync(stream, data, true);
            await ReadBytesFromStreamAsync(stream, data, 0, 1);

            return data[0];
        }

        public static async Task<(uint type, int size, long length)> ReadChunkPrefixAsync(Stream stream)
        {
            var size = PrefixSize;
            var available = stream.Length - stream.Position;

            if (PrefixSize > available)
            {
                return (AtomTypes.Empty, 0, -1);
            }

            var data = new byte[size];

            //await ReadBytesFromStreamAsync(stream, data, true);
            await ReadBytesFromStreamAsync(stream, data, 0, 4, true);
            await ReadBytesFromStreamAsync(stream, data, 4, 4, true);

            //var length = (long)BitConverter.ToUInt32(data.Slice(0, 4).ToBigEndian(), 0);
            var length = (long)BitConverter.ToUInt32(data, 0);
            //var atomType = BitConverter.ToUInt32(data.Slice(4).ToBigEndian(), 0);
            var atomType = BitConverter.ToUInt32(data, 4);

            if (1U == length)
            {
                size += PrefixSize;

                //await ReadBytesFromStreamAsync(stream, data, true);
                await ReadBytesFromStreamAsync(stream, data, 0, 8, true);

                length = BitConverter.ToInt64(data, 0);
            }

            return (atomType, size, length);
        }

        private static async Task ReadBytesFromStreamAsync(Stream stream, byte[] bytes, bool throwException = false)
        {
            var bufferLength = bytes.Length;
            var actualCount = await stream.ReadAsync(bytes, 0, bufferLength);

            if ((bufferLength != actualCount) && throwException)
            {
                throw new InvalidOperationException();
            }
        }

        private static async Task ReadBytesFromStreamAsync(Stream stream, byte[] buffer, int offset, int count)
        {
            var actual = await stream.ReadAsync(buffer, offset, count);

            if (actual != count)
            {
                throw new Exception();
            }
        }

        private static async Task ReadBytesFromStreamAsync(Stream stream, byte[] buffer, int offset, int count, bool forceEndian)
        {
            var actual = await stream.ReadAsync(buffer, offset, count);

            if (actual != count)
            {
                throw new Exception();
            }

            if (forceEndian)
            {
                ToBigEndian(buffer, offset, count);
            }
        }

        private static void ToBigEndian(byte[] buffer, int offset, int count)
        {
            if (false == BitConverter.IsLittleEndian)
            {
                return;
            }

            var iterations = count >> 1;
            int start = offset;
            int end = offset + count - 1;

            for (var index = 0; index < iterations; index++)
            {
                var temp = buffer[start];

                buffer[start++] = buffer[end];
                buffer[end--] = temp;
            }
        }

        private static async Task<byte[]> ReadBytesFromStreamBigEndianAsync(Stream stream, int count)
        {
            var buffer = new byte[count];

            await ReadBytesFromStreamAsync(stream, buffer, true);

            if (BitConverter.IsLittleEndian)
            {
                var length = buffer.Length;
                var iterations = length >> 1;
                int start = 0;
                int end = length - 1;

                for (var index = 0; index < iterations; index++)
                {
                    var place = buffer[start];

                    buffer[start++] = buffer[end];
                    buffer[end--] = place;
                }
            }

            return buffer;
        }
    }
}