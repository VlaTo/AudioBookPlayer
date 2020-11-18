namespace LibraProgramming.QuickTime.Container.Chunks
{
    /*
    [Chunk(AtomTypes.Stbl)]
    public class StblChunk : ContainerChunk
    {
        public StblChunk(Chunk[] chunks)
            : base(AtomTypes.Stbl, chunks)
        {
        }

        public new static StblChunk ReadFrom(Atom atom)
        {
            if (null == atom)
            {
                throw new ArgumentNullException(nameof(atom));
            }

            var chunks = new List<Chunk>();

            using (var extractor = new AtomExtractor(atom.Stream))
            {
                var factory = ChunkFactory.Instance;

                foreach (var child in extractor)
                {
                    var chunk = factory.CreateFrom(child);
                    chunks.Add(chunk);
                }
            }

            return new StblChunk(chunks.ToArray());
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