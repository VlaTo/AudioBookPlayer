using LibraProgramming.Media.QuickTime.Chunks;
using LibraProgramming.Media.QuickTime.Components;
using LibraProgramming.Media.QuickTime.Extensions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;

namespace LibraProgramming.Media.QuickTime
{
    public sealed class ChunkFactory
    {
        private readonly IDictionary<uint, Func<Atom, Chunk>> cache;

        public static readonly ChunkFactory Instance;

        private ChunkFactory(IDictionary<uint, Func<Atom, Chunk>> cache)
        {
            this.cache = cache;
        }

        static ChunkFactory()
        {
            var @namespace = typeof(Chunk).Namespace + ".Chunks";
            var dict = new Dictionary<uint, Func<Atom, Chunk>>();
            var types = typeof(ChunkFactory).Assembly
                .GetTypes()
                .Where(type => type.Namespace.StartsWith(@namespace))
                .ToArray();

            foreach (var type in types)
            {
                var attributes = type.GetCustomAttributes<ChunkAttribute>();

                foreach (var attribute in attributes)
                {
                    var func = GetCreator(type);
                    dict.Add(attribute.AtomType, func);
                }
            }

            Instance = new ChunkFactory(dict);
        }

        public Chunk CreateFrom(Atom atom)
        {
            if (cache.TryGetValue(atom.Type, out var creator))
            {
                return creator.Invoke(atom);
            }

            var bytes = BitConverter.GetBytes(atom.Type);
            var type = Encoding.ASCII.GetString(bytes.ToBigEndian());

            Debug.WriteLine($"public const uint {type.ToVariableName()} = 0x{atom.Type:X08};\t// chunk factory");

            return ContentChunk.ReadFrom(atom);
            //throw new ArgumentNullException(nameof(atom));
        }

        private static Func<Atom, Chunk> GetCreator(Type type)
        {
            var method = type.GetMethod("ReadFrom", BindingFlags.Static | BindingFlags.Public);
            return method.CreateDelegate<Func<Atom, Chunk>>();
        }
    }
}