using LibraProgramming.Media.QuickTime.Components;
using System;
using System.Collections.Generic;

namespace LibraProgramming.Media.QuickTime.Chunks
{
    [Chunk(AtomTypes.Mdia)]
    internal sealed class MdiaChunk : ContainerChunk
    {
        public MdiaChunk(Chunk[] chunks)
            : base(AtomTypes.Mdia, chunks)
        {
        }

        [ChunkCreator]
        public new static MdiaChunk ReadFrom(Atom atom)
        {
            if (null == atom)
            {
                throw new ArgumentNullException(nameof(atom));
            }

            var chunks = new List<Chunk>();

            using (var stream = new ReadOnlyAtomStream(atom.Stream, 0, atom.Stream.Length))
            {
                var extractor = new AtomExtractor(stream);

                foreach (var chuld in extractor)
                {
                    var chunk = ChunkFactory.Instance.CreateFrom(chuld);

                    switch (chunk)
                    {
                        case MdhdChunk mdhd:
                        {

                            break;
                        }

                        case HdlrChunk hdlr:
                        {

                            break;
                        }

                        case MinfChunk minf:
                        {

                            break;
                        }

                        default:
                        {

                            break;
                        }
                    }

                    chunks.Add(chunk);
                }
            }

            return new MdiaChunk(chunks.ToArray());
        }
    }
}