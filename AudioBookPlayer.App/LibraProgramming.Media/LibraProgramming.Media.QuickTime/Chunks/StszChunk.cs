using LibraProgramming.Media.QuickTime.Components;
using LibraProgramming.Media.QuickTime.Extensions;
using System;
using System.Text;

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

        public static StszChunk ReadFrom(Atom atom)
        {
            if (null == atom)
            {
                throw new ArgumentNullException(nameof(atom));
            }

            var (version, flags) = ReadFlagAndVersion(atom.Stream);
            var sampleSize = StreamHelper.ReadUInt32(atom.Stream);
            var numberOfSizes = StreamHelper.ReadUInt32(atom.Stream);
            var sampleSizes = new uint[numberOfSizes];

            for (var index = 0; index < numberOfSizes; index++)
            {
                var blockSize = StreamHelper.ReadUInt32(atom.Stream);
                sampleSizes[index] = blockSize;
            }

            return new StszChunk(sampleSize, sampleSizes);
        }
    }
}
