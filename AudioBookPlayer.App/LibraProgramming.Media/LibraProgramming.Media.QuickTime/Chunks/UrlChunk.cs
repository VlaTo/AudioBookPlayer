using LibraProgramming.Media.QuickTime.Components;
using System;
using System.Text;
using System.Threading.Tasks;

namespace LibraProgramming.Media.QuickTime.Chunks
{
    [Chunk(AtomTypes.Url)]
    internal sealed class UrlChunk : Chunk
    {
        private const uint InternalDataFlag = 0x000001;

        public string Url
        {
            get;
        }

        public UrlChunk(string url)
            : base(AtomTypes.Url)
        {
            Url = url;
        }

        [ChunkCreator]
        public static async Task<Chunk> ReadFromAsync(Atom atom)
        {
            if (null == atom)
            {
                throw new ArgumentNullException(nameof(atom));
            }

            var (_, flags) = await ReadFlagsAndVersionAsync(atom.Stream);
            var url = String.Empty;

            if ((flags & InternalDataFlag) == 0)
            {
                var length = (uint)(atom.Stream.Length - 4);

                if (0 < length)
                {
                    var bytes = await StreamHelper.ReadBytesAsync(atom.Stream, length);
                    var count = bytes.Length;

                    for (var index = count - 1; count >= 0; count--)
                    {
                        if (0x00 == bytes[index])
                        {
                            continue;
                        }

                        break;
                    }

                    if (0 < count)
                    {
                        url = Encoding.ASCII.GetString(bytes, 0, count);
                    }
                }
            }

            return new UrlChunk(url);
        }
    }
}
