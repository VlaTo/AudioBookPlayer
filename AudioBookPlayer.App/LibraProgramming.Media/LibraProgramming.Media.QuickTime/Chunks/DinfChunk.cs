namespace LibraProgramming.QuickTime.Container.Chunks
{
    /*[Chunk(AtomTypes.Dinf)]
    public class DinfChunk : ContainerChunk
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
                            break;
                        }

                        case MdiaChunk mdia:
                        {
                            break;
                        }
                    }

                    chunks.Add(chunk);
                }
            }

            return new DinfChunk(chunks.ToArray());
        }

        public override void Debug(int level)
        {
            var tabs = new String(' ', level);
            var bytes = BitConverter.GetBytes(Type).ToBigEndian();
            var type = Encoding.ASCII.GetString(bytes);

            Console.WriteLine($"{tabs}{type}");

            base.Debug(level);
        }
    }*/
}