using LibraProgramming.Media.QuickTime.Components;
using System;
using System.Collections.Generic;

namespace LibraProgramming.Media.QuickTime.Chunks
{
    [Chunk(AtomTypes.Meta)]
    internal class MetaChunk : ContainerChunk
    {
        public MetaChunk(Chunk[] chunks)
            : base(AtomTypes.Meta, chunks)
        {
        }

        public new static MetaChunk ReadFrom(Atom atom)
        {
            if (null == atom)
            {
                throw new ArgumentNullException(nameof(atom));
            }

            var bits = StreamHelper.ReadUInt32(atom.Stream);
            var version = (byte)((bits & 0xFF00_0000) >> 24);
            var flags = bits & 0x00FF_FFFF;

            var position = atom.Stream.Position;
            
            var chunks = new List<Chunk>();

            using (var source = new ReadOnlyAtomStream(atom.Stream, position, atom.Stream.Length - position))
            {
                var extractor = new AtomExtractor(source);

                foreach (var child in extractor)
                {
                    var chunk = ChunkFactory.Instance.CreateFrom(child);
                    chunks.Add(chunk);
                }
            }

            return new MetaChunk(chunks.ToArray());
        }
    }
}