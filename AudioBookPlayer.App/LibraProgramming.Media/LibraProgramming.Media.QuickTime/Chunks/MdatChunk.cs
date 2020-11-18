using LibraProgramming.Media.QuickTime.Components;

namespace LibraProgramming.Media.QuickTime.Chunks
{
    [Chunk(AtomTypes.Mdat)]
    internal sealed class MdatChunk : ContentChunk
    {
        public MdatChunk(long length)
            : base(AtomTypes.Mdat, length)
        {
        }

        public static new MdatChunk ReadFrom(Atom atom)
        {
            /*if (AtomTypes.Stsd == atom.Type)
            {
                var bytes = StreamHelper.ReadBytes(atom.Stream, (uint)atom.Stream.Length);
                Print.WriteDump(bytes, "STSD");
            }*/

            return new MdatChunk(atom.Stream.Length);
        }
    }
}
