using LibraProgramming.Media.QuickTime.Components;
using System;
using System.IO;
using System.Threading.Tasks;

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

        public static async Task<BlockDescription> ReadFromAsync(Stream stream)
        {
            var firstChunk = await StreamHelper.ReadUInt32Async(stream);
            var samplesPerChunk = await StreamHelper.ReadUInt32Async(stream);
            var sampleDurationIndex = await StreamHelper.ReadUInt32Async(stream);

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
        public static async Task<StscChunk> ReadFromAsync(Atom atom)
        {
            if (null == atom)
            {
                throw new ArgumentNullException(nameof(atom));
            }

            var (_, _) = await ReadFlagsAndVersionAsync(atom.Stream);
            var numberOfBlocks = await StreamHelper.ReadUInt32Async(atom.Stream);
            var blockSizes = new BlockDescription[numberOfBlocks];
            var position = atom.Stream.Position;

            using (var stream = new ReadOnlyAtomStream(atom.Stream, 0, atom.Stream.Length - position))
            {
                for (var index = 0; index < numberOfBlocks; index++)
                {
                    var block = await BlockDescription.ReadFromAsync(atom.Stream);
                    blockSizes[index] = block;
                }
            }

            return new StscChunk(blockSizes);
        }
    }
}
