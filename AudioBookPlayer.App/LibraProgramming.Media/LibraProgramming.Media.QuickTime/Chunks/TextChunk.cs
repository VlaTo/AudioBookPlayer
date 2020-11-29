using LibraProgramming.Media.QuickTime.Components;
using System;
using System.Threading.Tasks;

namespace LibraProgramming.Media.QuickTime.Chunks
{
    [Chunk(AtomTypes.Text)]
    internal sealed class TextChunk : Chunk
    {
        public TextChunk()
            : base(AtomTypes.Text)
        {
        }

        [ChunkCreator]
        public static async Task<Chunk> ReadFromAsync(Atom atom)
        {
            if (null == atom)
            {
                throw new ArgumentNullException(nameof(atom));
            }

            var reserved = await StreamHelper.ReadBytesAsync(atom.Stream, 6);
            var referenceIndex = await StreamHelper.ReadUInt16Async(atom.Stream); //dref index
            //var displayFlags = StreamHelper.ReadUInt32(atom.Stream);
            //var textJustification = StreamHelper.ReadUInt32(atom.Stream);

            return new TextChunk();
        }
    }
}
