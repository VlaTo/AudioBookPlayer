using LibraProgramming.Media.QuickTime.Components;
using System;
using System.IO;

namespace LibraProgramming.Media.QuickTime.Chunks
{
    internal sealed class BlockDescription
    {
        public uint FirstChunk
        {
            get;
        }

        public uint SamplesPerChunk
        {
            get;
        }

        public uint SampleDurationIndex
        {
            get;
        }

        public BlockDescription(uint firstChunk, uint samplesPerChunk, uint sampleDurationIndex)
        {
            FirstChunk = firstChunk;
            SamplesPerChunk = samplesPerChunk;
            SampleDurationIndex = sampleDurationIndex;
        }

        public static BlockDescription ReadFrom(Stream stream)
        {
            var firstChunk = StreamHelper.ReadUInt32(stream);
            var samplesPerChunk = StreamHelper.ReadUInt32(stream);
            var sampleDurationIndex = StreamHelper.ReadUInt32(stream);

            return new BlockDescription(firstChunk, samplesPerChunk, sampleDurationIndex);
        }
    }

    /// <summary>
    /// The sample-to-chunk atom.
    /// </summary>
    [Chunk(AtomTypes.Stsc)]
    internal sealed class StscChunk : Chunk
    {
        public BlockDescription[] BlockDescriptions
        {
            get;
        }

        public StscChunk(BlockDescription[] blockSizes)
            : base(AtomTypes.Stsc)
        {
            BlockDescriptions = blockSizes ?? Array.Empty<BlockDescription>();
        }

        [ChunkCreator]
        public static StscChunk ReadFrom(Atom atom)
        {
            if (null == atom)
            {
                throw new ArgumentNullException(nameof(atom));
            }

            var (_, _) = ReadFlagsAndVersion(atom.Stream);
            var numberOfBlocks = StreamHelper.ReadUInt32(atom.Stream);
            
            var position = atom.Stream.Position;
            
            var blockSizes = new BlockDescription[numberOfBlocks];

            using (var stream = new ReadOnlyAtomStream(atom.Stream, 0, atom.Stream.Length - position))
            {
                for (var index = 0; index < numberOfBlocks; index++)
                {
                    var block = BlockDescription.ReadFrom(atom.Stream);
                    blockSizes[index] = block;
                }
            }

            return new StscChunk(blockSizes);
        }
    }
}
