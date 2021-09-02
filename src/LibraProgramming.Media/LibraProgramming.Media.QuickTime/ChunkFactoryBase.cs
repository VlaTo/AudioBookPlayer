using LibraProgramming.Media.QuickTime.Components;
using LibraProgramming.Media.QuickTime.Extensions;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace LibraProgramming.Media.QuickTime
{
    internal class ChunkFactoryBase
    {
        private readonly IDictionary<uint, Func<Atom, Chunk>> cache;

        protected IDictionary<uint, Func<Atom, Chunk>> Cache => cache;

        protected ChunkFactoryBase(IDictionary<uint, Func<Atom, Chunk>> cache)
        {
            this.cache = cache;
        }

        protected static Func<Atom, Chunk> GetCreator(Type type)
        {
            var methods = type.GetMethods(BindingFlags.Static | BindingFlags.Public);

            foreach (var method in methods)
            {
                var attribute = method.GetCustomAttribute<ChunkCreatorAttribute>();

                if (null == attribute)
                {
                    continue;
                }

                if (IsFit(method))
                {
                    return method.CreateDelegate<Func<Atom, Chunk>>();
                }
            }

            throw new NotImplementedException();
        }
        
        protected static IDictionary<uint, Func<Atom, Chunk>> CreateCache(Type[] candidates)
        {
            var cache = new Dictionary<uint, Func<Atom, Chunk>>();

            foreach (var type in candidates)
            {
                var attributes = type.GetCustomAttributes<ChunkAttribute>();

                foreach (var attribute in attributes)
                {
                    var func = GetCreator(type);
                    cache.Add(attribute.AtomType, func);
                }
            }

            return cache;
        }
        
        private static bool IsFit(MethodInfo method)
        {
            if (typeof(Chunk).IsAssignableFrom(method.ReturnType))
            {
                var parameters = method.GetParameters();
                return 1 == parameters.Length && typeof(Atom).IsAssignableFrom(parameters[0].ParameterType);
            }

            return false;
        }
    }
}
