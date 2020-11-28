using LibraProgramming.Media.QuickTime.Components;
using System;
using System.Threading.Tasks;

namespace LibraProgramming.Media.QuickTime.Chunks
{
    /// <summary>
    /// The sample size atom.
    /// </summary>
    [Chunk(AtomTypes.Stsz)]
    internal sealed class StszChunk : Chunk
    {
        public uint SampleSize
        {
            get;
        }

        public uint[] SampleSizes
        {
            get;
        }

        public StszChunk(uint sampleSize, uint[] sampleSizes)
            : base(AtomTypes.Stsz)
        {
            SampleSize = sampleSize;
            SampleSizes = sampleSizes ?? Array.Empty<uint>();
        }

        [ChunkCreator]
        public static async Task<StszChunk> ReadFromAsync(Atom atom)
        {
            if (null == atom)
            {
                throw new ArgumentNullException(nameof(atom));
            }

            var (_, _) =await ReadFlagsAndVersionAsync(atom.Stream);
            var sampleSize = await StreamHelper.ReadUInt32Async(atom.Stream);
            var numberOfSizes = await StreamHelper.ReadUInt32Async(atom.Stream);
            var sampleSizes = new uint[numberOfSizes];

            for (var index = 0; index < numberOfSizes; index++)
            {
                var blockSize = await StreamHelper.ReadUInt32Async(atom.Stream);
                sampleSizes[index] = blockSize;
            }

            return new StszChunk(sampleSize, sampleSizes);
        }
    }
}
