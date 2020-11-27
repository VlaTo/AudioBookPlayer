using LibraProgramming.Media.QuickTime.Components;
using LibraProgramming.Media.QuickTime.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace LibraProgramming.Media.QuickTime.Chunks
{
    //[Chunk(AtomTypes.Dinf)]
    //[Chunk(AtomTypes.Mdia)]
    //[Chunk(AtomTypes.Minf)]
    //[Chunk(AtomTypes.Stbl)]
    //[Chunk(AtomTypes.Trak)]
    [Chunk(AtomTypes.Edts)]
    internal class ContainerChunk : Chunk
    {
        public Chunk[] Chunks
        {
            get;
        }

        public ContainerChunk(uint type, Chunk[] chunks)
            : base(type)
        {
            Chunks = chunks;
        }

        public static ContainerChunk ReadFrom(Atom atom)
        {
            if (null == atom)
            {
                throw new ArgumentNullException(nameof(atom));
            }

            var chunks = new List<Chunk>();
            var extractor = new AtomExtractor(atom.Stream);

            foreach (var child in extractor)
            {
                var chunk = ChunkFactory.Instance.CreateFrom(child);
                chunks.Add(chunk);
            }

            return new ContainerChunk(atom.Type, chunks.ToArray());
        }

        public override void Debug(int level)
        {
            var tabs = new String(' ', level);
            var bytes = BitConverter.GetBytes(Type).ToBigEndian();
            var type = Encoding.ASCII.GetString(bytes);

            Console.WriteLine($"{tabs}{type}");

            var next = level + 1;

            foreach (var chunk in Chunks)
            {
                chunk.Debug(next);
            }
        }
    }
}