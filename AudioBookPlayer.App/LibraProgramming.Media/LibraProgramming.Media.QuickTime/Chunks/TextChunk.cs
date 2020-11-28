using LibraProgramming.Media.QuickTime.Components;
using LibraProgramming.Media.QuickTime.Extensions;
using System;
using System.Text;

namespace LibraProgramming.Media.QuickTime.Chunks
{
    [Chunk(AtomTypes.Text)]
    internal sealed class TextChunk : Chunk
    {
        public TextChunk()
            : base(AtomTypes.Text)
        {
        }

        public static TextChunk ReadFrom(Atom atom)
        {
            if (null == atom)
            {
                throw new ArgumentNullException(nameof(atom));
            }

            var reserved = StreamHelper.ReadBytes(atom.Stream, 6);
            var referenceIndex = StreamHelper.ReadUInt16(atom.Stream); //dref index
            //var displayFlags = StreamHelper.ReadUInt32(atom.Stream);
            //var textJustification = StreamHelper.ReadUInt32(atom.Stream);

            return new TextChunk();
        }

        public override void Debug(int level)
        {
            var tabs = new String(' ', level);
            var bytes = BitConverter.GetBytes(Type).ToBigEndian();
            var type = Encoding.ASCII.GetString(bytes);

            Console.WriteLine($"{tabs}{type}");
        }
    }
}
