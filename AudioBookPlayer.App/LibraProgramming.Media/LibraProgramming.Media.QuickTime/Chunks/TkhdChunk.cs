using LibraProgramming.Media.QuickTime;
using LibraProgramming.Media.QuickTime.Components;
using LibraProgramming.Media.QuickTime.Extensions;
using System;
using System.Text;

namespace LibraProgramming.Media.QuickTime.Chunks
{
    [Chunk(AtomTypes.Tkhd)]
    internal sealed class TkhdChunk : Chunk
    {
        public bool IsPoster
        {
            get;
            private set;
        }

        public bool IsPreview
        {
            get;
            private set;
        }

        public bool IsMovie
        {
            get;
            private set;
        }

        public bool IsEnabled
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

        public int TrackId
        {
            get;
            private set;
        }

        private TkhdChunk()
            : base(AtomTypes.Tkhd)
        {
        }

        public static TkhdChunk ReadFrom(Atom atom)
        {
            if (null == atom)
            {
                throw new ArgumentNullException(nameof(atom));
            }

            var bits = StreamHelper.ReadUInt32(atom.Stream);
            var version = (byte) ((bits & Flag.VersionMask) >> 24);
            var poster = (bits & Flag.Poster) == Flag.Poster;
            var preview = (bits & Flag.Preview) == Flag.Preview;
            var movie = (bits & Flag.Movie) == Flag.Movie;
            var enabled = (bits & Flag.Enabled) == Flag.Enabled;
            DateTime created = ReadUtcDateTime(atom.Stream, version);
            DateTime modified = ReadUtcDateTime(atom.Stream, version);
            var trackId = StreamHelper.ReadInt32(atom.Stream);
            var reserved0 = StreamHelper.ReadUInt64(atom.Stream);

            /*switch (version)
            {
                case 0:
                {
                    var temp1 = StreamHelper.ReadUInt32(atom.Stream);
                    var temp2 = StreamHelper.ReadUInt32(atom.Stream);

                    created = DateTime.UtcNow;
                    modified = DateTime.UtcNow;

                    break;
                }

                case 1:
                {
                    var temp1 = StreamHelper.ReadUInt64(atom.Stream);
                    var temp2 = StreamHelper.ReadUInt64(atom.Stream);

                    created = DateTime.UtcNow;
                    modified = DateTime.UtcNow;

                    break;
                }

                default:
                {
                    throw new NotSupportedException();
                }
            }*/

            //var trackId = StreamHelper.ReadInt32(atom.Stream);
            //var reserved0 = StreamHelper.ReadUInt64(atom.Stream);

            TimeSpan duration;

            switch (version)
            {
                case 0:
                {
                    var temp1 = StreamHelper.ReadUInt32(atom.Stream);

                    duration = TimeSpan.Zero;

                    break;
                }

                case 1:
                {
                    var temp1 = StreamHelper.ReadUInt64(atom.Stream);

                    duration = TimeSpan.Zero;

                    break;
                }

                default:
                {
                    throw new NotSupportedException();
                }
            }

            var reserved1 = StreamHelper.ReadUInt32(atom.Stream);
            var videoLayer = StreamHelper.ReadUInt16(atom.Stream);

            return new TkhdChunk
            {
                TrackId = trackId,
                IsEnabled = enabled,
                IsPoster = poster,
                IsPreview = preview,
                IsMovie = movie,
                Duration = duration,
                Created = created,
                Modified = modified
            };
        }

        public override void Debug(int level)
        {
            var tabs = new String(' ', level);
            var bytes = BitConverter.GetBytes(Type);
            var type = Encoding.ASCII.GetString(bytes.ToBigEndian());

            Console.WriteLine($"{tabs}{type} track ID: '{TrackId}'");
        }

        private static class Flag
        {
            public const uint VersionMask = 0xFF00_0000;
            public const uint Poster = 0x0000_0008;
            public const uint Preview = 0x0000_0004;
            public const uint Movie = 0x0000_0002;
            public const uint Enabled = 0x0000_0001;
        }
    }
}