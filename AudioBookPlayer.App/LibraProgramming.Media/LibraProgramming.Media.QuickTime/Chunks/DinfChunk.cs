using LibraProgramming.Media.QuickTime.Components;
using LibraProgramming.Media.QuickTime.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace LibraProgramming.Media.QuickTime.Chunks
{
    /// <summary>
    /// The data information atom.
    /// </summary>
    [Chunk(AtomTypes.Dinf)]
    internal sealed class DinfChunk : ContainerChunk
    {
        public DinfChunk(Chunk[] chunks)
            : base(AtomTypes.Dinf, chunks)
        {
        }

        public new static DinfChunk ReadFrom(Atom atom)
        {
            if (null == atom)
            {
                throw new ArgumentNullException(nameof(atom));
            }

            var position = atom.Stream.Position;
            var chunks = new List<Chunk>();

            using (var source = new ReadOnlyAtomStream(atom.Stream, position, atom.Stream.Length - position))
            {
                var extractor = new AtomExtractor(source);

                foreach (var child in extractor)
                {
                    var chunk = ChunkFactory.Instance.CreateFrom(child);

                    switch (chunk.Type)
                    {
                        case AtomTypes.Url:
                        {
                            break;
                        }
                    }

                    chunks.Add(chunk);
                }
            }

            return new DinfChunk(chunks.ToArray());
        }

        /*public override void Debug(int level)
        {
            var tabs = new String(' ', level);
            var bytes = BitConverter.GetBytes(Type).ToBigEndian();
            var type = Encoding.ASCII.GetString(bytes);

            Console.WriteLine($"{tabs}{type}");

            base.Debug(level);
        }*/
    }
}