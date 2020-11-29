using LibraProgramming.Media.QuickTime.Components;
using LibraProgramming.Media.QuickTime.Extensions;
using LibraProgramming.Media.QuickTime.Lists;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LibraProgramming.Media.QuickTime.Chunks
{
    [Chunk(AtomTypes.Ilst)]
    internal class IlstChunk : Chunk
    {
        public MetaInfoChunk[] MetaInfoChunks
        {
            get;
        }

        public IlstChunk(MetaInfoChunk[] chunks)
            : base(AtomTypes.Ilst)
        {
            MetaInfoChunks = chunks;
        }

        [ChunkCreator]
        public static async Task<Chunk> ReadFromAsync(Atom atom)
        {
            if (null == atom)
            {
                throw new ArgumentNullException(nameof(atom));
            }

            var chunks = new List<MetaInfoChunk>();
            var extractor = new AtomExtractor(atom.Stream);

            using (var enumerator = extractor.GetEnumerator())
            {
                enumerator.Reset();

                while (await enumerator.MoveNextAsync())
                {
                    var chunk = await InformationFactory.Instance.CreateFromAsync(enumerator.Current);

                    switch (chunk)
                    {
                        case MetaInfoChunk meta:
                        {
                            chunks.Add(meta);
                            break;
                        }

                        default:
                        {

                            break;
                        }
                    }
                }
            }

            return new IlstChunk(chunks.ToArray());
        }

        public override void Debug(int level)
        {
            var tabs = new String(' ', level);
            var bytes = BitConverter.GetBytes(Type).ToBigEndian();
            var type = Encoding.ASCII.GetString(bytes);

            Console.WriteLine($"{tabs}{type}");

            var next = level + 1;

            foreach (var chunk in MetaInfoChunks)
            {
                chunk.Debug(next);
            }
        }
    }
}