using LibraProgramming.Media.QuickTime.Components;
using System;
using System.IO;

namespace LibraProgramming.Media.QuickTime.Chunks
{
    /// <summary>
    /// The sample size atom.
    /// </summary>
    [Chunk(AtomTypes.Stsz)]
    internal sealed class StszChunk : Chunk
    {
        public bool IsSameSize
        {
            get;
        }

        public uint SampleSize
        {
            get;
        }

        public uint[] SampleSizes
        {
            get;
        }

        private StszChunk(bool isSameSize, uint sampleSize, uint[] sampleSizes)
            : base(AtomTypes.Stsz)
        {
            IsSameSize = isSameSize;
            SampleSize = sampleSize;
            SampleSizes = sampleSizes ?? Array.Empty<uint>();
        }

        [ChunkCreator]
        public static StszChunk ReadFrom(Atom atom)
        {
            if (null == atom)
            {
                throw new ArgumentNullException(nameof(atom));
            }

            var (_, _) = ReadFlagsAndVersion(atom.Stream);
            var sampleSize = StreamHelper.ReadUInt32(atom.Stream);
            var numberOfSizes = StreamHelper.ReadUInt32(atom.Stream);

            if (0 < sampleSize)
            {
                return new StszChunk(true, sampleSize, null);
            }

            if (0 == sampleSize)
            {
                var length = atom.Stream.Length - atom.Stream.Position;
                var sampleSizes = new uint[numberOfSizes];

                // possible problem with start offset here as in Chunks\SttsChunk.cs:66
                using (var source = new ReadOnlyAtomStream(atom.Stream, 0L, length))
                {
                    var bufferSize = Math.Min((int)source.Length, 10240);
                    using (var stream = new BufferedStream(source, bufferSize))
                    {
                        for (var index = 0; index < numberOfSizes; index++)
                        {
                            var blockSize = StreamHelper.ReadUInt32(stream);
                            sampleSizes[index] = blockSize;
                        }
                    }
                }

                return new StszChunk(false, 0, sampleSizes);
            }

            throw new Exception();
        }
    }
}
