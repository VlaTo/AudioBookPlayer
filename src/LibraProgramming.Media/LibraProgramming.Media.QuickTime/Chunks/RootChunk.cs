using System;

namespace LibraProgramming.Media.QuickTime.Chunks
{
    internal sealed class RootChunk : ContainerChunk
    {
        public RootChunk(Chunk[] chunks)
            : base(AtomTypes.Root, chunks)
        {
        }

        public override void Debug(int level)
        {
            var tabs = new String(' ', level);

            Console.WriteLine($"{tabs}ROOT");

            var next = level + 1;

            foreach (var chunk in Chunks)
            {
                chunk.Debug(next);
            }
        }
    }
}
