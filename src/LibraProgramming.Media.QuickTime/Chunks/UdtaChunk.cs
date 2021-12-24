using LibraProgramming.Media.QuickTime.Components;
using System;
using System.Collections.Generic;

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
        public new static UdtaChunk ReadFrom(Atom atom)
        {
            if (null == atom)
            {
                throw new ArgumentNullException(nameof(atom));
            }

            var chunks = new List<Chunk>();
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
                }*/

                chunks.Add(chunk);
            }

            return new UdtaChunk(chunks.ToArray());
        }
    }
}