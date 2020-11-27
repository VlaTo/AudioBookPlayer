using LibraProgramming.Media.QuickTime.Components;
using System;
using System.Collections.Generic;

namespace LibraProgramming.Media.QuickTime.Chunks
{
    /// <summary>
    /// The data reference atom.
    /// </summary>
    [Chunk(AtomTypes.Dref)]
    internal class DrefChunk : ContainerChunk
    {
        private readonly uint flags;

        public byte Version
        {
            get;
        }

        public DrefChunk(byte version, uint flags, Chunk[] chunks)
            : base(AtomTypes.Dref, chunks)
        {
            this.flags = flags;

            Version = version;
        }

        public new static DrefChunk ReadFrom(Atom atom)
        {
            if (null == atom)
            {
                throw new ArgumentNullException(nameof(atom));
            }

            var (version, flags) = ReadFlagsAndVersion(atom.Stream);
            var numberOfReferences = StreamHelper.ReadUInt32(atom.Stream);
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
                    var chunk = ChunkFactory.Instance.CreateFrom(current);

                    chunks.Add(chunk);
                }
            }

            return new DrefChunk(version,flags, chunks.ToArray());
        }
    }
}