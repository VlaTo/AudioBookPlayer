using LibraProgramming.Media.QuickTime.Components;

namespace LibraProgramming.Media.QuickTime.Chunks
{
    [Chunk(AtomTypes.Free)]
    internal sealed class FreeChunk : ContentChunk
    {
        public FreeChunk(long length)
            : base(AtomTypes.Free, length)
        {
        }

        [ChunkCreator]
        public static new FreeChunk ReadFrom(Atom atom)
        {
            return new FreeChunk(atom.Stream.Length);
        }
    }
}
