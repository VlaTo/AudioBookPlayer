using LibraProgramming.Media.QuickTime.Components;
using LibraProgramming.Media.QuickTime.Extensions;
using System;
using System.IO;
using System.Text;

namespace LibraProgramming.Media.QuickTime.Chunks
{
    /// <summary>
    /// The handler reference atom.
    /// </summary>
    [Chunk(AtomTypes.Hdlr)]
    internal sealed class HdlrChunk : Chunk
    {
        public string ComponentType
        {
            get;
        }

        public string ComponentSubtype
        {
            get;
        }

        private HdlrChunk(string componentType, string componentSubtype)
            : base(AtomTypes.Hdlr)
        {
            ComponentType = componentType;
            ComponentSubtype = componentSubtype;
        }

        public static HdlrChunk ReadFrom(Atom atom)
        {
            if (null == atom)
            {
                throw new ArgumentNullException(nameof(atom));
            }

            var (version, flags) = ReadFlagAndVersion(atom.Stream);
            var componentType = ReadComponentString(atom.Stream);
            var componentSubtype = ReadComponentString(atom.Stream);
            var manufacturer = StreamHelper.ReadUInt32(atom.Stream);
            var quicktimeFlags = StreamHelper.ReadUInt32(atom.Stream);
            var quicktimeMask = StreamHelper.ReadUInt32(atom.Stream);
            var componentLength = StreamHelper.ReadByte(atom.Stream);
            var componentName = StreamHelper.ReadString(atom.Stream, componentLength);

            return new HdlrChunk(componentType, componentSubtype);
        }

        public override void Debug(int level)
        {
            var tabs = new String(' ', level);
            var bytes = BitConverter.GetBytes(Type);
            var type = Encoding.ASCII.GetString(bytes.ToBigEndian());

            Console.WriteLine($"{tabs}{type}");
        }

        private static string ReadComponentString(Stream stream)
        {
            var value = StreamHelper.ReadUInt32(stream);

            if (0x00000000 < value)
            {
                var bytes = BitConverter.GetBytes(value);
                return Encoding.ASCII.GetString(bytes, 0, bytes.Length);
            }

            return String.Empty;
        }
    }
}