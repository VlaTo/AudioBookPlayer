using LibraProgramming.Media.QuickTime.Components;
using LibraProgramming.Media.QuickTime.Extensions;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

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

        [ChunkCreator]
        public static async Task<HdlrChunk> ReadFromAsync(Atom atom)
        {
            if (null == atom)
            {
                throw new ArgumentNullException(nameof(atom));
            }

            var (version, flags) = await ReadFlagsAndVersionAsync(atom.Stream);
            var componentType = await ReadComponentStringAsync(atom.Stream);
            var componentSubtype = await ReadComponentStringAsync(atom.Stream);
            var manufacturer = await StreamHelper.ReadUInt32Async(atom.Stream);
            var quicktimeFlags = await StreamHelper.ReadUInt32Async(atom.Stream);
            var quicktimeMask = await StreamHelper.ReadUInt32Async(atom.Stream);
            var componentLength = await StreamHelper.ReadByteAsync(atom.Stream);
            var componentName = await StreamHelper.ReadStringAsync(atom.Stream, componentLength);

            return new HdlrChunk(componentType, componentSubtype);
        }

        public override void Debug(int level)
        {
            var tabs = new String(' ', level);
            var bytes = BitConverter.GetBytes(Type);
            var type = Encoding.ASCII.GetString(bytes.ToBigEndian());

            Console.WriteLine($"{tabs}{type}");
        }

        private static async Task<string> ReadComponentStringAsync(Stream stream)
        {
            var value = await StreamHelper.ReadUInt32Async(stream);

            if (0x00000000 < value)
            {
                var bytes = BitConverter.GetBytes(value);
                return Encoding.ASCII.GetString(bytes, 0, bytes.Length);
            }

            return String.Empty;
        }
    }
}