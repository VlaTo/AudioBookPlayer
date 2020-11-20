using LibraProgramming.Media.QuickTime.Components;
using LibraProgramming.Media.QuickTime.Extensions;
using System;
using System.Text;

namespace LibraProgramming.Media.QuickTime.Chunks
{
    [Chunk(AtomTypes.Stsz)]
    internal sealed class StszChunk : Chunk
    {
        public uint[] BlockSizes
        {
            get;
        }

        public StszChunk(uint[] blockSizes)
            : base(AtomTypes.Stsz)
        {
            BlockSizes = blockSizes ?? Array.Empty<uint>();
        }

        public static StszChunk ReadFrom(Atom atom)
        {
            if (null == atom)
            {
                throw new ArgumentNullException(nameof(atom));
            }

            var bits = StreamHelper.ReadUInt32(atom.Stream);
            var numberOfBlocks = StreamHelper.ReadUInt32(atom.Stream);

            var version = (byte)((bits & 0xFF00_0000) >> 24);
            var flags = bits & 0x00FF_FFFF;

            var blockSizes = new uint[numberOfBlocks];

            for (var index = 0; index < numberOfBlocks; index++)
            {
                var blockSize = StreamHelper.ReadUInt32(atom.Stream);
                blockSizes[index] = blockSize;
            }

            return new StszChunk(blockSizes);
        }

        public override void Debug(int level)
        {
            var tabs = new String(' ', level);
            var bytes = BitConverter.GetBytes(Type).ToBigEndian();
            var type = Encoding.ASCII.GetString(bytes);

            Console.WriteLine($"{tabs}{type} block sizes: {BlockSizes.Length}");
        }
    }
}
