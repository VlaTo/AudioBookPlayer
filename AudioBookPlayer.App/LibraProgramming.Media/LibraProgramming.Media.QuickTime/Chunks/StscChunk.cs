using LibraProgramming.Media.QuickTime.Components;
using LibraProgramming.Media.QuickTime.Extensions;
using System;
using System.Text;

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
    }

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

        public static StscChunk ReadFrom(Atom atom)
        {
            if (null == atom)
            {
                throw new ArgumentNullException(nameof(atom));
            }

            var bits = StreamHelper.ReadUInt32(atom.Stream);
            var numberOfBlocks = StreamHelper.ReadUInt32(atom.Stream);

            var version = (byte)((bits & 0xFF00_0000) >> 24);
            var flags = bits & 0x00FF_FFFF;

            var blockSizes = new BlockDescription[numberOfBlocks];

            for (var index = 0; index < numberOfBlocks; index++)
            {
                var firstChunk = StreamHelper.ReadUInt32(atom.Stream);
                var samplesPerChunk = StreamHelper.ReadUInt32(atom.Stream);
                var sampleDurationIndex = StreamHelper.ReadUInt32(atom.Stream);

                blockSizes[index] = new BlockDescription(firstChunk, samplesPerChunk, sampleDurationIndex);
            }

            return new StscChunk(blockSizes);
        }

        public override void Debug(int level)
        {
            var tabs = new String(' ', level);
            var bytes = BitConverter.GetBytes(Type).ToBigEndian();
            var type = Encoding.ASCII.GetString(bytes);

            Console.WriteLine($"{tabs}{type} blocks: {BlockDescriptions.Length}");
        }
    }
}
