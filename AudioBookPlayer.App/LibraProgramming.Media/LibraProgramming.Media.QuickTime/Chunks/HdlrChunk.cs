using LibraProgramming.Media.QuickTime.Components;
using LibraProgramming.Media.QuickTime.Extensions;
using System;
using System.Text;

namespace LibraProgramming.Media.QuickTime.Chunks
{
    [Chunk(AtomTypes.Hdlr)]
    internal sealed class HdlrChunk : Chunk
    {
        private HdlrChunk()
            : base(AtomTypes.Hdlr)
        {
        }

        public static HdlrChunk ReadFrom(Atom atom)
        {
            if (null == atom)
            {
                throw new ArgumentNullException(nameof(atom));
            }

            var bits = StreamHelper.ReadUInt32(atom.Stream);
            var version = (bits & 0xFF00_0000) >> 24;
            var flags = bits & 0x00FF_FFFF;
            var type = StreamHelper.ReadUInt32(atom.Stream);
            //var type = StreamHelper.ReadString(atom.Stream, 8);
            var subtype = StreamHelper.ReadUInt32(atom.Stream);
            //var subtype = StreamHelper.ReadString(atom.Stream, 8);
            var manufacturer = StreamHelper.ReadUInt32(atom.Stream);
            var quicktimeFlags = StreamHelper.ReadUInt32(atom.Stream);
            var quicktimeMask = StreamHelper.ReadUInt32(atom.Stream);
            var componentLength = StreamHelper.ReadByte(atom.Stream);
            var componentName = StreamHelper.ReadString(atom.Stream, componentLength);

            return new HdlrChunk();
        }

        public override void Debug(int level)
        {
            var tabs = new String(' ', level);
            var bytes = BitConverter.GetBytes(Type);
            var type = Encoding.ASCII.GetString(bytes.ToBigEndian());

            Console.WriteLine($"{tabs}{type}");
        }
    }
}