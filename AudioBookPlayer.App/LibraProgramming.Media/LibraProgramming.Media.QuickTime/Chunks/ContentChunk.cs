using LibraProgramming.Media.QuickTime.Components;
using LibraProgramming.Media.QuickTime.Extensions;
using System;
using System.Text;
using System.Threading.Tasks;

namespace LibraProgramming.Media.QuickTime.Chunks
{
    [Chunk(AtomTypes.Wide)]
    [Chunk(AtomTypes.Iods)]
    [Chunk(AtomTypes.Smhd)]
    [Chunk(AtomTypes.Gmhd)]
    [Chunk(AtomTypes.Stss)]
    [Chunk(AtomTypes.Ctts)]
    [Chunk(AtomTypes.Name)]
    [Chunk(AtomTypes.Elst)]
    [Chunk(AtomTypes.Vmhd)]
    [Chunk(AtomTypes.Hnti)]
    [Chunk(AtomTypes.Hinf)]
    [Chunk(AtomTypes.Sgpd)]
    [Chunk(AtomTypes.Sbgp)]
    [Chunk(AtomTypes.Chpl)]
    [Chunk(AtomTypes.Esds)]
    [Chunk(AtomTypes.Alis)]
    [Chunk(AtomTypes.Rtp)]
    [Chunk(AtomTypes.Mp4v)]
    internal class ContentChunk : Chunk
    {
        public long Length
        {
            get;
        }

        public ContentChunk(uint type, long length)
            : base(type)
        {
            Length = length;
        }

        public static Task<ContentChunk> ReadFromAsync(Atom atom)
        {
            /*if (AtomTypes.Stsd == atom.Type)
            {
                var bytes = StreamHelper.ReadBytes(atom.Stream, (uint)atom.Stream.Length);
                Print.WriteDump(bytes,"STSD");
            }*/

            return Task.FromResult(new ContentChunk(atom.Type, atom.Stream.Length));
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