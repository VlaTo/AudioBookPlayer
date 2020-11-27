using LibraProgramming.Media.QuickTime;
using LibraProgramming.Media.QuickTime.Components;
using LibraProgramming.Media.QuickTime.Extensions;
using System;
using System.IO;
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

        public ulong Duration
        {
            get;
            private set;
        }

        public uint TrackId
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

            var (version, flags) = ReadFlagsAndVersion(atom.Stream);
            var poster = (flags & Flag.Poster) == Flag.Poster;
            var preview = (flags & Flag.Preview) == Flag.Preview;
            var movie = (flags & Flag.Movie) == Flag.Movie;
            var enabled = (flags & Flag.Enabled) == Flag.Enabled;
            DateTime created = ReadUtcDateTime(atom.Stream, version);
            DateTime modified = ReadUtcDateTime(atom.Stream, version);
            var trackId = StreamHelper.ReadUInt32(atom.Stream);
            var reserved0 = StreamHelper.ReadUInt32(atom.Stream);
            var duration = ReadDuration(atom.Stream, version);
            var reserved1 = StreamHelper.ReadBytes(atom.Stream, 8);
            var layer = StreamHelper.ReadUInt16(atom.Stream);
            var alternateGroup = StreamHelper.ReadUInt16(atom.Stream);

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

        /*public override void Debug(int level)
        {
            var tabs = new String(' ', level);
            var bytes = BitConverter.GetBytes(Type);
            var type = Encoding.ASCII.GetString(bytes.ToBigEndian());

            Console.WriteLine($"{tabs}{type} track ID: '{TrackId}'");
        }*/

        private static ulong ReadDuration(Stream stream, byte version)
        {
            switch (version)
            {
                case 0:
                {
                    return (ulong) StreamHelper.ReadUInt32(stream);
                }

                case 1:
                {
                    return StreamHelper.ReadUInt64(stream);
                }

                default:
                {
                    throw new NotSupportedException();
                }
            }
        }

        private static class Flag
        {
            public const uint Enabled = 0x0000_0001;
            public const uint Movie = 0x0000_0002;
            public const uint Preview = 0x0000_0004;
            public const uint Poster = 0x0000_0008;
        }
    }
}