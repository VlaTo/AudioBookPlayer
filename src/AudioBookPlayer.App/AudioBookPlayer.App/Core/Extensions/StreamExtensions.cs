using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace AudioBookPlayer.App.Core.Extensions
{
    public static class StreamExtensions
    {
        internal const int ReadBufferSize = 4096;

        public static byte[] ToBytes(this Stream stream)
        {
            if (null == stream)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            var length = stream.Length;
            var data = new byte[length];
            var offset = 0;

            while (true)
            {
                var count = Math.Min((int) (length - offset), ReadBufferSize);

                if (0 == count)
                {
                    break;
                }

                var result = stream.Read(data, offset, count);

                offset += result;
            }

            return data;
        }
        
        public static async Task<byte[]> ToBytesAsync(this Stream stream, CancellationToken cancellation = default)
        {
            if (null == stream)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            var data = new byte[stream.Length];
            var offset = 0;

            int count;
            while (0 < (count = await stream.ReadAsync(data, offset, ReadBufferSize, cancellation)))
            {
                offset += count;
            }

            return data;
        }
    }
}