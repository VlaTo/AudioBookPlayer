using LibraProgramming.Media.QuickTime.Components;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

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

                            default:
                            {

                                break;
                            }
                        }

                        chunks.Add(chunk);
                    }
                }
            }

            return new StblChunk(chunks.ToArray());
        }
    }
}