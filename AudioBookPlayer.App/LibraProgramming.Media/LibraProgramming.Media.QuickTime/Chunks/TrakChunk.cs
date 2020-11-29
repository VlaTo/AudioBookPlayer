using LibraProgramming.Media.QuickTime.Components;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

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
        public new static async Task<Chunk> ReadFromAsync(Atom atom)
        {
            if (null == atom)
            {
                throw new ArgumentNullException(nameof(atom));
            }

            var chunks = new List<Chunk>();

            using (var source = new ReadOnlyAtomStream(atom.Stream, 0, atom.Stream.Length))
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
                }
            }

            return new TrakChunk(chunks.ToArray());
        }
    }
}