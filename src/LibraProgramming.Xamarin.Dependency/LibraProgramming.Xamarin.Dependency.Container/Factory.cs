using LibraProgramming.Xamarin.Dependency.Container.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace LibraProgramming.Xamarin.Dependency.Container
{
    public abstract class Factory
    {
        protected IInstanceProvider Provider
        {
            get;
        }

        public abstract object Create(Queue<ServiceTypeReference> types);

        protected Factory(IInstanceProvider provider)
        {
            Provider = provider;
        }

        protected ConstructorInfo GetConstructor(Type type)
        {
            var ti = type.GetTypeInfo();
            var candidate = ti.DeclaredConstructors
                .FirstOrDefault(ctor => null != ctor.GetCustomAttribute<PrefferedConstructorAttribute>());

            if (candidate != null)
            {
                return candidate;
            }

            foreach (var ctor in ti.DeclaredConstructors)
            {
                var args = ctor.GetParameters();

                if (false == args.Any())
                {
                    continue;
                }

                if (args.Select(arg => arg.ParameterType).All(arg => arg.GetTypeInfo().IsInterface))
                {
                    return ctor;
                }
            }

            return ti.DeclaredConstructors.First(ctor => ctor.IsPublic);
            //            return ti.DeclaredConstructors.First();
        }

        protected object CreateInstance(Queue<ServiceTypeReference> types, Type type)
        {
            var ctor = GetConstructor(type);

            if (null != ctor)
            {
                var args = ctor.GetParameters();

                if (args.Length == 0)
                {
                    return ctor.Invoke(new object[0]);
                }

                var values = new object[args.Length];

                foreach (var arg in args)
                {
                    var attribute = arg.GetCustomAttribute<ServiceAttribute>();
                    var service = new ServiceTypeReference(arg.ParameterType, attribute?.Key);

                    if (types.Contains(service))
                    {
                        Throw.CyclicServiceReference(type, arg.Name, service.Type, service.Key);
                    }

                    types.Enqueue(service);

                    try
                    {
                        values[arg.Position] = Provider.GetInstance(types, service.Type, service.Key);
                    }
                    finally
                    {
                        types.Dequeue();
                    }
                }

                return ctor.Invoke(values);
            }

            throw new Exception();

        }
    }
}
