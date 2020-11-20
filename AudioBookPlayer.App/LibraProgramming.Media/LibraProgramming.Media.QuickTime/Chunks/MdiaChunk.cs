using LibraProgramming.Media.QuickTime.Components;
using LibraProgramming.Media.QuickTime.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace LibraProgramming.Media.QuickTime.Chunks
{
    [Chunk(AtomTypes.Mdia)]
    internal sealed class MdiaChunk : ContainerChunk
    {
        public MdiaChunk(Chunk[] chunks)
            : base(AtomTypes.Mdia, chunks)
        {
        }

        public new static MdiaChunk ReadFrom(Atom atom)
        {
            if (null == atom)
            {
                throw new ArgumentNullException(nameof(atom));
            }

            var chunks = new List<Chunk>();
            var extractor = new AtomExtractor(atom.Stream);

            foreach (var chuld in extractor)
            {
                var chunk = ChunkFactory.Instance.CreateFrom(chuld);

                switch (chunk)
                {
                    case TkhdChunk tkhd:
                    {

                        break;
                    }
                }

                chunks.Add(chunk);
            }

            return new MdiaChunk(chunks.ToArray());
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