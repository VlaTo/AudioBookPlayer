using LibraProgramming.Media.QuickTime.Components;
using System.Threading.Tasks;

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
        public static new Task<Chunk> ReadFromAsync(Atom atom)
        {
            return Task.FromResult<Chunk>(new MdatChunk(atom.Stream.Length));
        }
    }
}
