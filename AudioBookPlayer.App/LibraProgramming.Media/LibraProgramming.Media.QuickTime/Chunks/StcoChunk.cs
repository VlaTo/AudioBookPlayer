using LibraProgramming.Media.QuickTime.Components;
using LibraProgramming.Media.QuickTime.Extensions;
using System;
using System.Text;

namespace LibraProgramming.Media.QuickTime.Chunks
{
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

        public static StcoChunk ReadFrom(Atom atom)
        {
            if (null == atom)
            {
                throw new ArgumentNullException(nameof(atom));
            }

            var bits = StreamHelper.ReadUInt32(atom.Stream);
            var numberOfOffsets = StreamHelper.ReadUInt32(atom.Stream);
            
            var version = (byte)((bits & 0xFF00_0000) >> 24);
            var flags = bits & 0x00FF_FFFF;

            var offsets = new uint[numberOfOffsets];

            for (var index = 0; index < numberOfOffsets; index++)
            {
                var offset = StreamHelper.ReadUInt32(atom.Stream);
                offsets[index] = offset;
            }

            return new StcoChunk(offsets);
        }

        public override void Debug(int level)
        {
            var tabs = new String(' ', level);
            var bytes = BitConverter.GetBytes(Type).ToBigEndian();
            var type = Encoding.ASCII.GetString(bytes);

            Console.WriteLine($"{tabs}{type} offsets: {Offsets.Length}");
        }
    }
}
