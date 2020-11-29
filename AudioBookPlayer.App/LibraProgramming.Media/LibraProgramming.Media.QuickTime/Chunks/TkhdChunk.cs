using LibraProgramming.Media.QuickTime;
using LibraProgramming.Media.QuickTime.Components;
using LibraProgramming.Media.QuickTime.Extensions;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

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

        [ChunkCreator]
        public static async Task<Chunk> ReadFromAsync(Atom atom)
        {
            if (null == atom)
            {
                throw new ArgumentNullException(nameof(atom));
            }

            var (version, flags) = await ReadFlagsAndVersionAsync(atom.Stream);
            var poster = (flags & Flag.Poster) == Flag.Poster;
            var preview = (flags & Flag.Preview) == Flag.Preview;
            var movie = (flags & Flag.Movie) == Flag.Movie;
            var enabled = (flags & Flag.Enabled) == Flag.Enabled;
            var created = await ReadUtcDateTimeAsync(atom.Stream, version);
            var modified = await ReadUtcDateTimeAsync(atom.Stream, version);
            var trackId = await StreamHelper.ReadUInt32Async(atom.Stream);
            var reserved0 = await StreamHelper.ReadUInt32Async(atom.Stream);
            var duration = await ReadDurationAsync(atom.Stream, version);
            var reserved1 = await StreamHelper.ReadBytesAsync(atom.Stream, 8);
            var layer = await StreamHelper.ReadUInt16Async(atom.Stream);
            var alternateGroup = await StreamHelper.ReadUInt16Async(atom.Stream);

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

        private static async Task<ulong> ReadDurationAsync(Stream stream, byte version)
        {
            switch (version)
            {
                case 0:
                {
                    return (ulong) await StreamHelper.ReadUInt32Async(stream);
                }

                case 1:
                {
                    return await StreamHelper.ReadUInt64Async(stream);
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