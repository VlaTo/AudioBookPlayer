using LibraProgramming.Media.QuickTime.Components;
using LibraProgramming.Media.QuickTime.Extensions;
using System;
using System.Text;

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

            var (version, flags) = ReadFlagsAndVersion(atom.Stream);
            var numberOfOffsets = StreamHelper.ReadUInt32(atom.Stream);
            var offsets = new uint[numberOfOffsets];

            for (var index = 0; index < numberOfOffsets; index++)
            {
                var offset = StreamHelper.ReadUInt32(atom.Stream);
                offsets[index] = offset;
            }

            return new StcoChunk(offsets);
        }
        
        /*public override void Debug(int level)
        {
            var tabs = new String(' ', level);
            var bytes = BitConverter.GetBytes(Type).ToBigEndian();
            var type = Encoding.ASCII.GetString(bytes);

            Console.WriteLine($"{tabs}{type} (offsets: {Offsets.Length})");

            var count = Offsets.Length;

            Console.WriteLine($"{tabs} index     offsets");

            for (var index = 0; index < Math.Min(3, count); index++)
            {
                var offset = Offsets[index];
                Console.WriteLine($"{tabs}[{index:d8}] {offset:d8}");
            }

            if (3 < count)
            {
                var offset = Offsets[count - 1];
                Console.WriteLine($"{tabs}...");
                Console.WriteLine($"{tabs}[{(count - 1):d8}] {offset:d8}");
            }

            Console.WriteLine();
        }*/
    }
}
