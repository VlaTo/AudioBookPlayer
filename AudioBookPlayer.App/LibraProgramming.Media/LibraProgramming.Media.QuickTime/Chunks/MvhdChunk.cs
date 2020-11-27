using LibraProgramming.Media.QuickTime.Components;
using LibraProgramming.Media.QuickTime.Extensions;
using System;
using System.IO;
using System.Text;

namespace LibraProgramming.Media.QuickTime.Chunks
{
    [Chunk(AtomTypes.Mvhd)]
    internal sealed class MvhdChunk : Chunk
    {
        public uint Version
        {
            get;
            private set;
        }

        public uint PlaybackSpeed
        {
            get;
            private set;
        }

        public ushort Volume
        {
            get;
            private set;
        }

        public ulong PreviewIndex
        {
            get;
            private set;
        }

        public uint PosterIndex
        {
            get;
            private set;
        }

        public int NextId
        {
            get;
            private set;
        }

        public TimeSpan SelectionTime
        {
            get;
            private set;
        }

        public DateTime Created
        {
            get;
            private set;
        }

        public DateTime Modified
        {
            get;
            private set;
        }

        public TimeSpan Duration
        {
            get;
            private set;
        }

        private MvhdChunk()
            : base(AtomTypes.Mvhd)
        {
        }

        public static MvhdChunk ReadFrom(Atom atom)
        {
            if (null == atom)
            {
                throw new ArgumentNullException(nameof(atom));
            }

            var (version, flags) = ReadFlagsAndVersion(atom.Stream);
            //var bits = StreamHelper.ReadFlags32(atom.Stream);
            //var version = (bits & 0xFF00_0000) >> 24;
            //var flags = bits & 0x00FF_FFFF;

            DateTime created = ReadUtcDateTime(atom.Stream, version);
            DateTime modified = ReadUtcDateTime(atom.Stream, version);
            TimeSpan duration;

            /*switch (version)
            {
                case 0:
                {
                    var value = StreamHelper.ReadUInt32(atom.Stream);
                    
                    created = new DateTime(1904, 1, 1, 0, 0, 0, DateTimeKind.Utc) + TimeSpan.FromSeconds(value);

                    value = StreamHelper.ReadUInt32(atom.Stream);
                    modified = DateTime.UtcNow;

                    break;
                }

                case 1:
                {
                    var value = StreamHelper.ReadInt64(atom.Stream);
                    created = DateTimeOffset.FromUnixTimeMilliseconds(value).ToUniversalTime().DateTime;

                    value = StreamHelper.ReadInt64(atom.Stream);
                    modified = DateTime.UtcNow;

                    break;
                }

                default:
                {
                    throw new NotSupportedException();
                }
            }*/

            var timeScale = StreamHelper.ReadUInt32(atom.Stream);

            switch (version)
            {
                case 0:
                {
                    var value = StreamHelper.ReadUInt32(atom.Stream);

                    duration = TimeSpan.FromSeconds(value / timeScale);

                    break;
                }

                case 1:
                {
                    var value = StreamHelper.ReadUInt64(atom.Stream);

                    duration = TimeSpan.FromSeconds(value / timeScale);

                    break;
                }

                default:
                {
                    throw new NotSupportedException();
                }
            }

            var playbackSpeed = StreamHelper.ReadUInt32(atom.Stream);
            var volume = StreamHelper.ReadUInt16(atom.Stream);
            var reserved1 = StreamHelper.ReadUInt16(atom.Stream);
            var wgm = Wgm.Read(atom.Stream);
            var preview = StreamHelper.ReadUInt64(atom.Stream);
            var poster = StreamHelper.ReadUInt32(atom.Stream);
            var selectionTime = StreamHelper.ReadInt64(atom.Stream);
            var currentTime = ReadUtcDateTime(atom.Stream, 1); //StreamHelper.ReadInt64(atom.Stream);
            var nextId = StreamHelper.ReadInt32(atom.Stream);

            return new MvhdChunk
            {
                Version = version,
                PlaybackSpeed = playbackSpeed,
                Volume = volume,
                Duration = duration,
                PreviewIndex = preview,
                PosterIndex = poster,
                NextId = nextId,
                SelectionTime = TimeSpan.FromMilliseconds(selectionTime),
                Created = created,
                Modified = modified
            };
        }

        public override void Debug(int level)
        {
            var tabs = new String(' ', level);
            var bytes = BitConverter.GetBytes(Type);
            var type = Encoding.ASCII.GetString(bytes.ToBigEndian());

            Console.WriteLine($"{tabs}{type} duration: '{Duration:g}'");
        }

        /// <summary>
        /// 
        /// </summary>
        public sealed class Wgm
        {
            public uint A
            {
                get;
            }

            public uint B
            {
                get;
            }

            public uint U
            {
                get;
            }

            public uint C
            {
                get;
            }

            public uint D
            {
                get;
            }

            public uint V
            {
                get;
            }

            public uint X
            {
                get;
            }

            public uint Y
            {
                get;
            }

            public uint W
            {
                get;
            }

            private Wgm(uint a, uint b, uint u, uint c, uint d, uint v, uint x, uint y, uint w)
            {
                A = a;
                B = b;
                U = u;
                C = c;
                D = d;
                V = v;
                X = x;
                Y = y;
                W = w;
            }

            public static Wgm Read(Stream stream)
            {
                var a = StreamHelper.ReadUInt32(stream);
                var b = StreamHelper.ReadUInt32(stream);
                var u = StreamHelper.ReadUInt32(stream);
                var c = StreamHelper.ReadUInt32(stream);
                var d = StreamHelper.ReadUInt32(stream);
                var v = StreamHelper.ReadUInt32(stream);
                var x = StreamHelper.ReadUInt32(stream);
                var y = StreamHelper.ReadUInt32(stream);
                var w = StreamHelper.ReadUInt32(stream);

                return new Wgm(a, b, u, c, d, v, x, y, w);
            }
        }
    }
}