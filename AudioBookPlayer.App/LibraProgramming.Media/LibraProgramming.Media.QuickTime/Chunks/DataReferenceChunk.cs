using LibraProgramming.Media.QuickTime.Components;
using System;
using System.Collections.Generic;

namespace LibraProgramming.Media.QuickTime.Chunks
{
    [Chunk(AtomTypes.Dref)]
    internal class DataReferenceChunk : ContainerChunk
    {
        private readonly uint flags;

        public byte Version
        {
            get;
        }

        public DataReferenceChunk(byte version, uint flags, Chunk[] chunks)
            : base(AtomTypes.Dref, chunks)
        {
            this.flags = flags;

            Version = version;
        }

        public new static DataReferenceChunk ReadFrom(Atom atom)
        {
            if (null == atom)
            {
                throw new ArgumentNullException(nameof(atom));
            }

            var bits = StreamHelper.ReadUInt32(atom.Stream);
            var numberOfReferences = StreamHelper.ReadUInt32(atom.Stream);
            var position = atom.Stream.Position;

            var version = (byte)((bits & 0xFF00_0000) >> 24);
            var flags = bits & 0x00FF_FFFF;

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

            return new DataReferenceChunk(version,flags, chunks.ToArray());
        }
    }
}