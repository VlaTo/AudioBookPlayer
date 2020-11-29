using LibraProgramming.Media.QuickTime.Components;

namespace LibraProgramming.Media.QuickTime.Chunks
{
    /// <summary>
    /// The media data atom.
    /// </summary>
    [Chunk(AtomTypes.Mdat)]
    internal sealed class MdatChunk : ContentChunk
    {
        public MdatChunk(long length)
            : base(AtomTypes.Mdat, length)
        {
        }

        [ChunkCreator]
        public static new MdatChunk ReadFrom(Atom atom)
        {
            return new MdatChunk(atom.Stream.Length);
        }
    }
}
