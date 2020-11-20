using LibraProgramming.Media.QuickTime.Components;
using System;
using System.Collections.Generic;

namespace LibraProgramming.Media.QuickTime.Chunks
{
    [Chunk(AtomTypes.Stbl)]
    internal sealed class StblChunk : ContainerChunk
    {
        public StblChunk(Chunk[] chunks)
            : base(AtomTypes.Stbl, chunks)
        {
        }

        public new static StblChunk ReadFrom(Atom atom)
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
                
                switch (chunk)
                {
                    case StsdChunk stsd:
                    {

                        break;
                    }
                }

                chunks.Add(chunk);
            }

            return new StblChunk(chunks.ToArray());
        }

        /*public override void Debug(int level)
        {
            var tabs = new String(' ', level);
            var bytes = BitConverter.GetBytes(Type).ToBigEndian();
            var type = Encoding.ASCII.GetString(bytes);

            Console.WriteLine($"{tabs}{type}");

            base.Debug(level);
        }*/
    }
}