using LibraProgramming.Media.QuickTime.Components;
using System.Threading.Tasks;

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
        public static new Task<Chunk> ReadFromAsync(Atom atom)
        {
            return Task.FromResult<Chunk>(new FreeChunk(atom.Stream.Length));
        }
    }
}
