using LibraProgramming.Media.QuickTime.Components;
using System;

namespace LibraProgramming.Media.QuickTime.Chunks
{
    /// <summary>
    /// The generic media info header atom.
    /// </summary>
    [Chunk(AtomTypes.Gmhd)]
    internal sealed class GmhdChunk : Chunk
    {
        long Length
        {
            get;
        }

        private GmhdChunk(long length)
            : base(AtomTypes.Gmhd)
        {
            Length = length;
        }

        [ChunkCreator]
        public static GmhdChunk ReadFrom(Atom atom)
        {
            if (null == atom)
            {
                throw new ArgumentNullException(nameof(atom));
            }

            return new GmhdChunk(atom.Stream.Length);
        }
    }
}
