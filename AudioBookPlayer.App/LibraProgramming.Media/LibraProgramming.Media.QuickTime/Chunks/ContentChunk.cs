using LibraProgramming.Media.QuickTime.Components;
using LibraProgramming.Media.QuickTime.Extensions;
using System;
using System.Text;

namespace LibraProgramming.Media.QuickTime.Chunks
{
    [Chunk(AtomTypes.Wide)]
    [Chunk(AtomTypes.Iods)]
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

        [ChunkCreator]
        public static ContentChunk ReadFrom(Atom atom)
        {
            return new ContentChunk(atom.Type, atom.Stream.Length);
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