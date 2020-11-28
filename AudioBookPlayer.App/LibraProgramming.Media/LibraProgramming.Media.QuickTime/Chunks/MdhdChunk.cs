using LibraProgramming.Media.QuickTime.Components;
using System;
using System.Threading.Tasks;

namespace LibraProgramming.Media.QuickTime.Chunks
{
    /// <summary>
    /// The media header atom.
    /// </summary>
    [Chunk(AtomTypes.Mdhd)]
    internal sealed class MdhdChunk : Chunk
    {
        public uint SampleRate
        {
            get;
            private set;
        }

        public ulong TrackLength
        {
            get;
            private set;
        }

        public int Language
        {
            get;
            private set;
        }

        public ushort Quality
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

        public MdhdChunk()
            : base(AtomTypes.Mdhd)
        {
        }

        [ChunkCreator]
        public static async Task<MdhdChunk> ReadFromAsync(Atom atom)
        {
            if (null == atom)
            {
                throw new ArgumentNullException(nameof(atom));
            }

            var (version, flags) = await ReadFlagsAndVersionAsync(atom.Stream);
            var created = await ReadUtcDateTimeAsync(atom.Stream, version);
            var modified = await ReadUtcDateTimeAsync(atom.Stream, version);
            var sampleRate = await StreamHelper.ReadUInt32Async(atom.Stream);
            var trackLength = await ReadLengthAsync(atom.Stream, version);
            var language = await StreamHelper.ReadUInt16Async(atom.Stream) & 0x7FFF;
            var quality = await StreamHelper.ReadUInt16Async(atom.Stream);

            return new MdhdChunk
            {
                SampleRate = sampleRate,
                TrackLength = trackLength,
                Language = language,
                Quality = quality,
                Created = created,
                Modified = modified
            };
        }
    }
}
