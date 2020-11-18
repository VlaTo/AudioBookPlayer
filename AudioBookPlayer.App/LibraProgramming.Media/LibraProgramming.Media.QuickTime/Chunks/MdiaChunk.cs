namespace LibraProgramming.QuickTime.Container.Chunks
{
    /*
    [Chunk(AtomTypes.Mdia)]
    public class MdiaChunk : ContainerChunk
    {
        public MdiaChunk(Chunk[] chunks)
            : base(AtomTypes.Mdia, chunks)
        {
        }

        public new static MdiaChunk ReadFrom(Atom atom)
        {
            if (null == atom)
            {
                throw new ArgumentNullException(nameof(atom));
            }

            var chunks = new List<Chunk>();

            using (var extractor = new AtomExtractor(atom.Stream))
            {
                var factory = ChunkFactory.Instance;

                foreach (var chuld in extractor)
                {
                    var chunk = factory.CreateFrom(chuld);

                    switch (chunk)
                    {
                        case TkhdChunk tkhd:
                        {
                            //chunks.Add(tkhd);

                            break;
                        }
                    }

                    chunks.Add(chunk);
                }
            }

            return new MdiaChunk(chunks.ToArray());
        }

        public override void Debug(int level)
        {
            var tabs = new String(' ', level);
            var bytes = BitConverter.GetBytes(Type).ToBigEndian();
            var type = Encoding.ASCII.GetString(bytes);

            Console.WriteLine($"{tabs}{type}");

            base.Debug(level);
        }
    }
    */
}