using LibraProgramming.Media.QuickTime.Components;

namespace LibraProgramming.Media.QuickTime.Chunks
{
    /// <summary>
    /// The media data atom.
    /// </summary>
    [Chunk(AtomTypes.Mdat)]
    internal sealed class MdatChunk : ContentChunk
    {
        public long Start
        {
            get;
        }

        public MdatChunk(long start, long length)
            : base(AtomTypes.Mdat, length)
        {
            Start = start;
        }

        [ChunkCreator]
        public static new MdatChunk ReadFrom(Atom atom)
        {
            return new MdatChunk(atom.Stream.Start, atom.Stream.Length);
        }
    }
}
