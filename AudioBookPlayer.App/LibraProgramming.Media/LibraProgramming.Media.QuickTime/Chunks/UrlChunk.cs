using LibraProgramming.Media.QuickTime.Components;
using System;
using System.Collections.Generic;
using System;
using System.Collections.Generic;
using System.Text;
using LibraProgramming.Media.QuickTime.Extensions;

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

        public static UrlChunk ReadFrom(Atom atom)
        {
            if (null == atom)
            {
                throw new ArgumentNullException(nameof(atom));
            }

            var (version, flags) = ReadFlagsAndVersion(atom.Stream);
            var url = String.Empty;

            if ((flags & InternalDataFlag) == 0)
            {
                var length = (uint)(atom.Stream.Length - 4);

                if (0 < length)
                {
                    var bytes = StreamHelper.ReadBytes(atom.Stream, length);
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

        public override void Debug(int level)
        {
            var tabs = new String(' ', level);
            var bytes = BitConverter.GetBytes(Type).ToBigEndian();
            var type = Encoding.ASCII.GetString(bytes);

            Console.WriteLine($"{tabs}{type} [ulr: '{Url}']");
        }
    }
}
