using LibraProgramming.Media.QuickTime.Components;
using System;
using System.IO;
using System.Threading.Tasks;

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

        public uint TimeScale
        {
            get;
            private set;
        }

        public decimal PlaybackSpeed
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

        [ChunkCreator]
        public static async Task<MvhdChunk> ReadFromAsync(Atom atom)
        {
            if (null == atom)
            {
                throw new ArgumentNullException(nameof(atom));
            }

            var (version, _) = await ReadFlagsAndVersionAsync(atom.Stream);

            var created = await ReadUtcDateTimeAsync(atom.Stream, version);
            var modified = await ReadUtcDateTimeAsync(atom.Stream, version);
            var timeScale = await StreamHelper.ReadUInt32Async(atom.Stream);
            var duration = await ReadDurationAsync(atom.Stream, timeScale, version);
            var playbackSpeed = await StreamHelper.ReadInt32Async(atom.Stream);
            var volume = await StreamHelper.ReadUInt16Async(atom.Stream);
            var reserved1 = await StreamHelper.ReadUInt16Async(atom.Stream);
            var wgm = await StreamHelper.ReadBytesAsync(atom.Stream, 36);
            var previewTime = await StreamHelper.ReadUInt32Async(atom.Stream);
            var previewDuration = await StreamHelper.ReadUInt32Async(atom.Stream);
            var posterTime = await StreamHelper.ReadUInt32Async(atom.Stream);
            var selectionTime = await StreamHelper.ReadInt32Async(atom.Stream);
            var selectionDuration = await StreamHelper.ReadInt32Async(atom.Stream);
            var currentTime = await StreamHelper.ReadInt32Async(atom.Stream);
            var nextId = await StreamHelper.ReadInt32Async(atom.Stream);

            return new MvhdChunk
            {
                Version = version,
                TimeScale = timeScale,
                //PlaybackSpeed = BitConverter.ToSingle(playbackSpeed,0),
                PlaybackSpeed = new Decimal(playbackSpeed),
                Volume = volume,
                Duration = duration,
                //PreviewIndex = 0UL,
                //PosterIndex = poster,
                NextId = nextId,
                //SelectionTime = TimeSpan.FromMilliseconds(selectionTime),
                Created = created,
                Modified = modified
            };
        }

        private static async Task<TimeSpan> ReadDurationAsync(Stream stream, uint scale, byte version)
        {
            switch (version)
            {
                case 0:
                {
                    var value = await StreamHelper.ReadUInt32Async(stream);
                    return TimeSpan.FromSeconds(value / scale);
                }

                case 1:
                {
                    var value = await StreamHelper.ReadUInt64Async(stream);
                    return TimeSpan.FromSeconds(value / scale);
                }

                default:
                {
                    throw new NotSupportedException();
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /*public sealed class Wgm
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
        }*/
    }
}