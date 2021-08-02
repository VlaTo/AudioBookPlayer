using LibraProgramming.Media.QuickTime.Chunks;
using LibraProgramming.Media.QuickTime.Extensions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace LibraProgramming.Media.QuickTime
{
    internal sealed class InformationFactory : ChunkFactoryBase
    {
        public static readonly InformationFactory Instance;

        private InformationFactory(IDictionary<uint, Func<Atom, Chunk>> cache)
            : base(cache)
        {
        }

        static InformationFactory()
        {
            var @namespace = typeof(InformationFactory).Namespace + ".Lists";
            var types = typeof(InformationFactory).Assembly
                .GetTypes()
                .Where(type => type.Namespace.StartsWith(@namespace))
                .ToArray();

            Instance = new InformationFactory(CreateCache(types));
        }

        public Chunk CreateFrom(Atom atom)
        {
            if (Cache.TryGetValue(atom.Type, out var creator))
            {
                return creator.Invoke(atom);
            }

            var bytes = BitConverter.GetBytes(atom.Type).ToBigEndian();

            if (0xA9 == bytes[0])
            {
                var temp = new byte[bytes.Length - 1];

                for (int index = 0; index < temp.Length; index++)
                {
                    temp[index] = bytes[index + 1];
                }

                bytes = temp;
            }

            var type = Encoding.ASCII.GetString(bytes);

            Debug.WriteLine($"public const uint {type.ToVariableName()} = 0x{atom.Type:X08};\t// information factory");

            return ContentChunk.ReadFrom(atom);
        }
    }
}