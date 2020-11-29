using LibraProgramming.Media.QuickTime.Chunks;
using LibraProgramming.Media.QuickTime.Extensions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraProgramming.Media.QuickTime
{
    internal sealed class ChunkFactory : ChunkFactoryBase
    {
        public static readonly ChunkFactory Instance;

        private ChunkFactory(IDictionary<uint, Func<Atom, Task<Chunk>>> cache)
            : base(cache)
        {
        }

        static ChunkFactory()
        {
            var @namespace = typeof(Chunk).Namespace + ".Chunks";
            //var dict = new Dictionary<uint, Func<Atom, Task<Chunk>>>();
            var types = typeof(ChunkFactory).Assembly
                .GetTypes()
                .Where(type => type.Namespace.StartsWith(@namespace))
                .ToArray();

            /*foreach (var type in types)
            {
                var attributes = type.GetCustomAttributes<ChunkAttribute>();

                foreach (var attribute in attributes)
                {
                    var func = GetCreator(type);
                    dict.Add(attribute.AtomType, func);
                }
            }*/

            Instance = new ChunkFactory(CreateCache(types));
        }

        public async Task<Chunk> CreateFromAsync(Atom atom)
        {
            if (Cache.TryGetValue(atom.Type, out var creator))
            {
                return await creator.Invoke(atom);
            }

            var bytes = BitConverter.GetBytes(atom.Type);
            var type = Encoding.ASCII.GetString(bytes.ToBigEndian());

            Debug.WriteLine($"public const uint {type.ToVariableName()} = 0x{atom.Type:X08};\t// chunk factory");

            return await ContentChunk.ReadFromAsync(atom);
        }

        /*private static Func<Atom, Task<Chunk>> GetCreator(Type type)
        {
            var methods = type.GetMethods(BindingFlags.Static | BindingFlags.Public);

            foreach(var method in methods)
            {
                var attribute = method.GetCustomAttribute<ChunkCreatorAttribute>();

                if (null == attribute)
                {
                    continue;
                }

                if (typeof(Task<>).IsAssignableFrom(method.ReturnType))
                {
                    var parameters = method.GetParameters();

                    if (1 == parameters.Length && parameters[0].ParameterType.IsAssignableFrom(typeof(Atom)))
                    {
                        return method.CreateDelegate<Func<Atom, Task<Chunk>>>();
                    }
                }
            }

            //var method = type.GetMethod("ReadFrom", BindingFlags.Static | BindingFlags.Public);
            //return method.CreateDelegate<Func<Atom, Chunk>>();

            throw new NotImplementedException();
        }*/
    }
}