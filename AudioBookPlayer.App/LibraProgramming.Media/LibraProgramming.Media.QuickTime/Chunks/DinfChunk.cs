using LibraProgramming.Media.QuickTime.Components;
using System;
using System.Collections.Generic;

namespace LibraProgramming.Media.QuickTime.Chunks
{
    /// <summary>
    /// The data information atom.
    /// </summary>
    [Chunk(AtomTypes.Dinf)]
    internal sealed class DinfChunk : ContainerChunk
    {
        public DinfChunk(Chunk[] chunks)
            : base(AtomTypes.Dinf, chunks)
        {
        }

        public new static DinfChunk ReadFrom(Atom atom)
        {
            if (null == atom)
            {
                throw new ArgumentNullException(nameof(atom));
            }

            var chunks = new List<Chunk>();

            using (var source = new ReadOnlyAtomStream(atom.Stream, 0, atom.Stream.Length))
            {
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

                        case MdiaChunk mdia:
                        {

                            break;
                        }
                    }

                    chunks.Add(chunk);
                }
            }

            return new DinfChunk(chunks.ToArray());
        }
    }
}