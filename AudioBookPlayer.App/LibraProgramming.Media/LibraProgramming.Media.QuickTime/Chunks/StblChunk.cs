using LibraProgramming.Media.QuickTime.Components;
using System;
using System.Collections.Generic;

namespace LibraProgramming.Media.QuickTime.Chunks
{
    /// <summary>
    /// The sample table atom.
    /// </summary>
    [Chunk(AtomTypes.Stbl)]
    internal sealed class StblChunk : ContainerChunk
    {
        public StblChunk(Chunk[] chunks)
            : base(AtomTypes.Stbl, chunks)
        {
        }

        [ChunkCreator]
        public new static StblChunk ReadFrom(Atom atom)
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

                    switch (chunk)
                    {
                        case StsdChunk stsd:
                        {

                            break;
                        }

                        case SttsChunk stts:
                        {

                            break;
                        }

                        case StszChunk stsz:
                        {

                            break;
                        }

                        case StscChunk stsc:
                        {

                            break;
                        }

                        case StcoChunk stco:
                        {

                            break;
                        }

                        /*case CttsChunk:
                        {

                            break;
                        }*/

                        default:
                        {

                            break;
                        }
                    }

                    chunks.Add(chunk);
                }
            }

            return new StblChunk(chunks.ToArray());
        }
    }
}