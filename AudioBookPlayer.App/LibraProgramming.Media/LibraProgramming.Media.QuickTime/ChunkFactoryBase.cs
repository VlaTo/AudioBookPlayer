using LibraProgramming.Media.QuickTime.Components;
using LibraProgramming.Media.QuickTime.Extensions;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

namespace LibraProgramming.Media.QuickTime
{
    internal class ChunkFactoryBase
    {
        private readonly IDictionary<uint, Func<Atom, Task<Chunk>>> cache;

        protected IDictionary<uint, Func<Atom, Task<Chunk>>> Cache => cache;

        protected ChunkFactoryBase(IDictionary<uint, Func<Atom, Task<Chunk>>> cache)
        {
            this.cache = cache;
        }

        protected static Func<Atom, Task<Chunk>> GetCreator(Type type)
        {
            var methods = type.GetMethods(BindingFlags.Static | BindingFlags.Public);

            foreach (var method in methods)
            {
                var attribute = method.GetCustomAttribute<ChunkCreatorAttribute>();

                if (null == attribute)
                {
                    continue;
                }

                if (IsTemp(method))
                {
                    return method.CreateDelegate<Func<Atom, Task<Chunk>>>();
                }
            }

            throw new NotImplementedException();
        }

        protected static IDictionary<uint, Func<Atom, Task<Chunk>>> CreateCache(Type[] candidates)
        {
            var cache = new Dictionary<uint, Func<Atom, Task<Chunk>>>();

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

        private static bool IsTemp(MethodInfo method)
        {
            if (IsReturnType(method.ReturnType))
            {
                var parameters = method.GetParameters();
                return 1 == parameters.Length && parameters[0].ParameterType.IsAssignableFrom(typeof(Atom));
            }

            return false;
        }

        private static bool IsReturnType(Type returnType)
        {
            if (typeof(Task).IsAssignableFrom(returnType) && returnType.IsGenericType)
            {
                var arguments = returnType.GetGenericArguments();
                return 1 == arguments.Length && typeof(Chunk).IsAssignableFrom(arguments[0]);
            }

            return false;
        }
    }
}
