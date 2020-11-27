using LibraProgramming.Media.QuickTime.Extensions;
using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace LibraProgramming.Media.QuickTime
{
    [DebuggerTypeProxy(typeof(DisplayProxy))]
    public class Atom
    {
        public uint Type
        {
            get;
        }

        public ReadOnlyAtomStream Stream
        {
            get;
        }

        internal Atom(AtomHeader header, Stream stream)
        {
            Type = header.Type;
            Stream = new ReadOnlyAtomStream(stream, header.Offset + header.Length, header.ChunkLength - header.Length);
        }

        /*[Conditional("DEBUG")]
        public void Print()
        {
            System.Console.WriteLine($"[{Stream.Start:X8}:{Stream.Length:X8}] Chunk: '{Type}'");
        }*/

        private sealed class DisplayProxy
        {
            public uint Type
            {
                get;
            }

            public string Sign
            {
                get;
            }

            public DisplayProxy(Atom atom)
            {
                var bytes = BitConverter.GetBytes(atom.Type).ToBigEndian();
                
                if (bytes.Length > 1 && bytes[0] == 0xA9)
                {
                    bytes = bytes.Slice(1);
                }

                Type = atom.Type;
                Sign = Encoding.ASCII.GetString(bytes);
            }
        }
    }
}