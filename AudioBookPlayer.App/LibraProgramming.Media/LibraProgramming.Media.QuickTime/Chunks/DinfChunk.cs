using LibraProgramming.Media.QuickTime.Components;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

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
                var extractor = new AtomExtractor(source);

                using (var enumerator = extractor.GetEnumerator())
                {
                    enumerator.Reset();

                    while (await enumerator.MoveNextAsync())
                    {
                        var chunk = await ChunkFactory.Instance.CreateFromAsync(enumerator.Current);

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
            }

            return new DinfChunk(chunks.ToArray());
        }
    }
}