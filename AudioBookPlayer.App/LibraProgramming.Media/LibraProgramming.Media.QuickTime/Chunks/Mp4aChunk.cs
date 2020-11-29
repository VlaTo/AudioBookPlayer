using LibraProgramming.Media.QuickTime.Components;
using LibraProgramming.Media.QuickTime.Extensions;
using System;
using System.Text;
using System.Threading.Tasks;

namespace LibraProgramming.Media.QuickTime.Chunks
{
    [Chunk(AtomTypes.Mp4a)]
    internal class Mp4aChunk : Chunk
    {
        public ushort Version
        {
            get;
        }

        public ushort Revision
        {
            get;
        }

        public uint Vendor
        {
            get;
        }

        public Mp4aChunk(ushort version, ushort revision, uint vendor, Chunk esds)
            : base(AtomTypes.Mp4a)
        {
            Version = version;
            Revision = revision;
            Vendor = vendor;
        }

        [ChunkCreator]
        public static async Task<Chunk> ReadFromAsync(Atom atom)
        {
            if (null == atom)
            {
                throw new ArgumentNullException(nameof(atom));
            }

            //var bytes = StreamHelper.ReadBytes(atom.Stream, (uint)atom.Stream.Length);
            //Print.WriteDump(bytes, "MP4A");

            var bits = await StreamHelper.ReadUInt64Async(atom.Stream);
            var referenceIndex = (ushort) (bits & 0x0000_0000_0000_FFFF);
            var version = await StreamHelper.ReadUInt16Async(atom.Stream);
            var revision = await StreamHelper.ReadUInt16Async(atom.Stream);
            var vendor = await StreamHelper.ReadUInt32Async(atom.Stream);
            var channels = await StreamHelper.ReadUInt16Async(atom.Stream);
            var bps = await StreamHelper.ReadUInt16Async(atom.Stream);
            var compression = await StreamHelper.ReadUInt16Async(atom.Stream);
            var audioPacketSize = await StreamHelper.ReadUInt16Async(atom.Stream);
            var sampleRate = await StreamHelper.ReadUInt32Async(atom.Stream);

            var header = await AtomHeader.ReadFromAsync(atom.Stream, atom.Stream.Position);
            Chunk esds = null;

            if (null != header)
            {
                var factory = ChunkFactory.Instance;
                var chunk = new Atom(header, atom.Stream);

                if (AtomTypes.Esds == chunk.Type)
                {
                    esds = await factory.CreateFromAsync(chunk);
                }
            }

            /*var position = atom.Stream.Position;

            var chunks = new List<Chunk>();

            using (var source = new ReadOnlyAtomStream(atom.Stream, position, atom.Stream.Length - position))
            {
                using (var extractor = new AtomExtractor(source))
                {
                    var factory = ChunkFactory.Instance;
                    var enumerator = extractor.GetEnumerator();

                    enumerator.Reset();

                    for (var index = 0; index < 1 && enumerator.MoveNext(); index++)
                    {
                        var current = enumerator.Current;
                        var chunk = factory.CreateFrom(current);

                        chunks.Add(chunk);
                    }
                }
            }*/

            return new Mp4aChunk(version, revision, vendor, esds);
        }

        public override void Debug(int level)
        {
            var tabs = new String(' ', level);
            var bytes = BitConverter.GetBytes(Type).ToBigEndian();
            var type = Encoding.ASCII.GetString(bytes);

            Console.WriteLine($"{tabs}{type}");
        }
    }
}