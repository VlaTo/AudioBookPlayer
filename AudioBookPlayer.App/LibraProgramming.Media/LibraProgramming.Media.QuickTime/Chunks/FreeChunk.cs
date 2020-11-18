using LibraProgramming.Media.QuickTime.Components;

namespace LibraProgramming.Media.QuickTime.Chunks
{
    [Chunk(AtomTypes.Free)]
    internal sealed class FreeChunk : ContentChunk
    {
        public FreeChunk(long length)
            : base(AtomTypes.Free, length)
        {
        }

        public static new FreeChunk ReadFrom(Atom atom)
        {
            if (AtomTypes.Free == atom.Type)
            {
                //var bytes = StreamHelper.ReadBytes(atom.Stream, (uint)atom.Stream.Length);
                //Print.WriteDump(bytes, "STSD");
            }

            return new FreeChunk(atom.Stream.Length);
        }
    }
}
