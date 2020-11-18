using LibraProgramming.Media.QuickTime.Components;
using LibraProgramming.Media.QuickTime.Extensions;
using System;
using System.Text;

namespace LibraProgramming.Media.QuickTime.Lists
{
    [Chunk(AtomTypes.Art)]
    [Chunk(AtomTypes.Alb)]
    [Chunk(AtomTypes.Day)]
    [Chunk(AtomTypes.Gen)]
    [Chunk(AtomTypes.Cmt)]
    [Chunk(AtomTypes.Too)]
    [Chunk(AtomTypes.Covr)]
    [Chunk(AtomTypes.Nam)]
    [Chunk(AtomTypes.Stik)]
    public class MetaInfoChunk : Chunk
    {
        public DataChunk DataChunk
        {
            get;
        }

        public MetaInfoChunk(uint type, DataChunk dataChunk)
            : base(type)
        {
            DataChunk = dataChunk;
        }

        public static MetaInfoChunk ReadFrom(Atom atom)
        {
            if (null == atom)
            {
                throw new ArgumentNullException(nameof(atom));
            }

            DataChunk dataChunk = null;
            var extractor = new AtomExtractor(atom.Stream);

            foreach (var child in extractor)
            {
                var chunk = InformationFactory.Instance.CreateFrom(child);

                switch (chunk)
                {
                    case DataChunk data:
                    {
                        if (null != dataChunk)
                        {
                            throw new Exception();
                        }

                        dataChunk = data;

                        break;
                    }

                    default:
                    {
                        break;
                    }
                }
            }

            return new MetaInfoChunk(atom.Type, dataChunk);
        }

        public override void Debug(int level)
        {
            var tabs = new String(' ', level);
            var bytes = BitConverter.GetBytes(Type);
            var type = Encoding.ASCII.GetString(bytes.ToBigEndian());

            Console.WriteLine($"{tabs}{type}");
        }
    }
}