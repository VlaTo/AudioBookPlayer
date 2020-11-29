using LibraProgramming.Media.QuickTime.Components;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LibraProgramming.Media.QuickTime.Chunks
{
    /// <summary>
    /// The sample description atom.
    /// </summary>
    [Chunk(AtomTypes.Stsd)]
    internal class StsdChunk : ContainerChunk
    {
        public byte Version
        {
            get;
        }

        public StsdChunk(byte version, Chunk[] chunks)
            : base(AtomTypes.Stsd, chunks)
        {
            Version = version;
        }

        [ChunkCreator]
        public new static async Task<Chunk> ReadFromAsync(Atom atom)
        {
            if (null == atom)
            {
                throw new ArgumentNullException(nameof(atom));
            }

            var (version, _) = await ReadFlagsAndVersionAsync(atom.Stream);
            var numberOfReferences = await StreamHelper.ReadUInt32Async(atom.Stream);
            var position = atom.Stream.Position;

            var chunks = new List<Chunk>();

            using (var source = new ReadOnlyAtomStream(atom.Stream, position, atom.Stream.Length - position))
            {
                var extractor = new AtomExtractor(source);
                var enumerator = extractor.GetEnumerator();

                enumerator.Reset();

                for (var index = 0; index < numberOfReferences && await enumerator.MoveNextAsync(); index++)
                {
                    var current = enumerator.Current;
                    var chunk = await ChunkFactory.Instance.CreateFromAsync(current);

                    chunks.Add(chunk);
                }
            }

            return new StsdChunk(version, chunks.ToArray());
        }
    }
}