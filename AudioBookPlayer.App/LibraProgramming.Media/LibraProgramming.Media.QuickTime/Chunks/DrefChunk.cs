using LibraProgramming.Media.QuickTime.Components;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LibraProgramming.Media.QuickTime.Chunks
{
    /// <summary>
    /// The data reference atom.
    /// </summary>
    [Chunk(AtomTypes.Dref)]
    internal class DrefChunk : ContainerChunk
    {
        public DrefChunk(Chunk[] chunks)
            : base(AtomTypes.Dref, chunks)
        {
        }

        [ChunkCreator]
        public static new async Task<DrefChunk> ReadFromAsync(Atom atom)
        {
            if (null == atom)
            {
                throw new ArgumentNullException(nameof(atom));
            }

            var (_, _) = await ReadFlagsAndVersionAsync(atom.Stream);
            var numberOfReferences = await StreamHelper.ReadUInt32Async(atom.Stream);
            var position = atom.Stream.Position;

            var chunks = new List<Chunk>();

            using (var source = new ReadOnlyAtomStream(atom.Stream, position, atom.Stream.Length - position))
            {
                var extractor = new AtomExtractor(source);
                var enumerator = extractor.GetEnumerator();

                enumerator.Reset();

                for (var index = 0; index < numberOfReferences && enumerator.MoveNext(); index++)
                {
                    var current = enumerator.Current;
                    var chunk = await ChunkFactory.Instance.CreateFromAsync(current);

                    chunks.Add(chunk);
                }
            }

            return new DrefChunk(chunks.ToArray());
        }
    }
}