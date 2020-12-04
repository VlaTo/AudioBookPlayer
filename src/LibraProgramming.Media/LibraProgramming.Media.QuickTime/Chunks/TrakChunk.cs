using LibraProgramming.Media.QuickTime.Components;
using System;
using System.Collections.Generic;

namespace LibraProgramming.Media.QuickTime.Chunks
{
    [Chunk(AtomTypes.Trak)]
    internal sealed class TrakChunk : ContainerChunk
    {
        public TrakChunk(Chunk[] chunks)
            : base(AtomTypes.Trak, chunks)
        {
        }

        [ChunkCreator]
        public new static TrakChunk ReadFrom(Atom atom)
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
                    case TkhdChunk tkhd:
                    {

                        break;
                    }

                    case MdiaChunk mdia:
                    {

                        break;
                    }

                    case UdtaChunk udta:
                    {

                        break;
                    }
                }

                chunks.Add(chunk);
            }

            return new TrakChunk(chunks.ToArray());
        }
    }
}