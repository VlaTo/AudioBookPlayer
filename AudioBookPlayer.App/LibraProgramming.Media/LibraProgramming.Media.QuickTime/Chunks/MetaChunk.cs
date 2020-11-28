using LibraProgramming.Media.QuickTime.Components;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LibraProgramming.Media.QuickTime.Chunks
{
    [Chunk(AtomTypes.Meta)]
    internal class MetaChunk : ContainerChunk
    {
        public MetaChunk(Chunk[] chunks)
            : base(AtomTypes.Meta, chunks)
        {
        }

        [ChunkCreator]
        public static new async Task<MetaChunk> ReadFromAsync(Atom atom)
        {
            if (null == atom)
            {
                throw new ArgumentNullException(nameof(atom));
            }

            var (_, _) = await ReadFlagsAndVersionAsync(atom.Stream);
            var position = atom.Stream.Position;
            
            var chunks = new List<Chunk>();

            using (var source = new ReadOnlyAtomStream(atom.Stream, position, atom.Stream.Length - position))
            {
                var extractor = new AtomExtractor(source);

                foreach (var child in extractor)
                {
                    var chunk = await ChunkFactory.Instance.CreateFromAsync(child);
                    chunks.Add(chunk);
                }
            }

            return new MetaChunk(chunks.ToArray());
        }
    }
}