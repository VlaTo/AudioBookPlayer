using LibraProgramming.Media.QuickTime.Components;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

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
        public new static async Task<Chunk> ReadFromAsync(Atom atom)
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

                using (var enumerator = extractor.GetEnumerator())
                {
                    enumerator.Reset();

                    while (await enumerator.MoveNextAsync())
                    {
                        var chunk = await ChunkFactory.Instance.CreateFromAsync(enumerator.Current);

                        switch (chunk)
                        {
                            case StblChunk stbl:
                            {

                                break;
                            }
                        }

                        chunks.Add(chunk);
                    }
                }
            }

            return new MinfChunk(chunks.ToArray());
        }
    }
}