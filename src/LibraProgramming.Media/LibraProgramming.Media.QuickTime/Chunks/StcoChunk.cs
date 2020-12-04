using LibraProgramming.Media.QuickTime.Components;
using System;

namespace LibraProgramming.Media.QuickTime.Chunks
{
    /// <summary>
    /// The chunk offset atom.
    /// </summary>
    [Chunk(AtomTypes.Stco)]
    internal sealed class StcoChunk : Chunk
    {
        public uint[] Offsets
        {
            get;
        }

        public StcoChunk(uint[] offsets)
            : base(AtomTypes.Stco)
        {
            Offsets = offsets ?? Array.Empty<uint>();
        }

        [ChunkCreator]
        public static StcoChunk ReadFrom(Atom atom)
        {
            if (null == atom)
            {
                throw new ArgumentNullException(nameof(atom));
            }

            var (_, _) = ReadFlagsAndVersion(atom.Stream);
            var numberOfOffsets = StreamHelper.ReadUInt32(atom.Stream);
            
            var position = atom.Stream.Position;
            
            var offsets = new uint[numberOfOffsets];
            
            using (var stream = new ReadOnlyAtomStream(atom.Stream, 0, atom.Stream.Length - position))
            {
                for (var index = 0; index < numberOfOffsets; index++)
                {
                    var offset = StreamHelper.ReadUInt32(atom.Stream);
                    offsets[index] = offset;
                }
            }

            return new StcoChunk(offsets);
        }
    }
}
