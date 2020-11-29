using LibraProgramming.Media.QuickTime.Components;
using System;
using System.Collections.Generic;

namespace LibraProgramming.Media.QuickTime.Chunks
{
    [Chunk(AtomTypes.Minf)]
    internal sealed class MinfChunk : ContainerChunk
    {
        public MinfChunk(Chunk[] chunks)
            : base(AtomTypes.Minf, chunks)
        {
        }

        [ChunkCreator]
        public new static MinfChunk ReadFrom(Atom atom)
        {
            if (null == atom)
            {
                throw new ArgumentNullException(nameof(atom));
            }

            var position = atom.Stream.Position;
            var chunks = new List<Chunk>();

            using (var source = new ReadOnlyAtomStream(atom.Stream, position, atom.Stream.Length - position))
            {
                var extractor = new AtomExtractor(atom.Stream);

                foreach (var child in extractor)
                {
                    var chunk = ChunkFactory.Instance.CreateFrom(child);

                    switch (chunk)
                    {
                        /*case TkhdChunk tkhd:
                        {
                            break;
                        }*/

                        case StblChunk stbl:
                        {

                            break;
                        }
                    }

                    chunks.Add(chunk);
                }
            }

            return new MinfChunk(chunks.ToArray());
        }
    }
}