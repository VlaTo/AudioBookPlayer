using LibraProgramming.Media.QuickTime.Components;
using System;

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

        public static MdhdChunk ReadFrom(Atom atom)
        {
            if (null == atom)
            {
                throw new ArgumentNullException(nameof(atom));
            }

            var (version, flags) = ReadFlagsAndVersion(atom.Stream);
            var created = ReadUtcDateTime(atom.Stream, version);
            var modified = ReadUtcDateTime(atom.Stream, version);
            var sampleRate = StreamHelper.ReadUInt32(atom.Stream);
            var trackLength = ReadLength(atom.Stream, version);
            var language = StreamHelper.ReadUInt16(atom.Stream) & 0x7FFF;
            var quality = StreamHelper.ReadUInt16(atom.Stream);

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

        /*public override void Debug(int level)
        {
            var tabs = new String(' ', level);
            var bytes = BitConverter.GetBytes(Type).ToBigEndian();
            var type = Encoding.ASCII.GetString(bytes);

            Console.WriteLine($"{tabs}{type} [track length: {TrackLength}, sample rate: {SampleRate}, quality: {Quality}]");
        }*/
    }
}
