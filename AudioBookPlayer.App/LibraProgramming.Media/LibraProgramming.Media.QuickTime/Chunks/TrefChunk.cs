using LibraProgramming.Media.QuickTime.Components;
using System;
using System.Collections.Generic;

namespace LibraProgramming.Media.QuickTime.Chunks
{
    [Chunk(AtomTypes.Tref)]
    internal sealed class TrefChunk : ContainerChunk
    {
        public TrefChunk(Chunk[] chunks)
            : base(AtomTypes.Tref, chunks)
        {
        }

        public new static TrefChunk ReadFrom(Atom atom)
        {
            if (null == atom)
            {
                throw new ArgumentNullException(nameof(atom));
            }

            var chunks = new List<Chunk>();

            using (var source = new ReadOnlyAtomStream(atom.Stream, 0, atom.Stream.Length))
            {
                var extractor = new AtomExtractor(atom.Stream);

                foreach (var child in extractor)
                {
                    var chunk = ChunkFactory.Instance.CreateFrom(child);

                    /*switch (chunk)
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
                    }*/

                    chunks.Add(chunk);
                }
            }

            return new TrefChunk(chunks.ToArray());
        }
    }
}
