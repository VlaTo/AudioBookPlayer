using LibraProgramming.Media.QuickTime.Components;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LibraProgramming.Media.QuickTime.Chunks
{
    /// <summary>
    /// The User data atom.
    /// </summary>
    [Chunk(AtomTypes.Udta)]
    internal class UdtaChunk : ContainerChunk
    {
        public UdtaChunk(Chunk[] chunks)
            : base(AtomTypes.Udta, chunks)
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
            var extractor = new AtomExtractor(atom.Stream);

            using (var enumerator = extractor.GetEnumerator())
            {
                enumerator.Reset();

                while (await enumerator.MoveNextAsync())
                {
                    var chunk = await ChunkFactory.Instance.CreateFromAsync(enumerator.Current);

                    chunks.Add(chunk);
                }
            }

            return new UdtaChunk(chunks.ToArray());
        }
    }
}