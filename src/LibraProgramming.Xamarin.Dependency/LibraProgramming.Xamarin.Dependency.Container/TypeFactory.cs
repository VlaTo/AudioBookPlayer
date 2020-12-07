using System;
using System.Collections.Generic;

namespace LibraProgramming.Xamarin.Dependency.Container
{
    internal sealed class TypeFactory : Factory
    {
        private readonly Type type;
        
        public TypeFactory(IInstanceProvider provider, Type type)
            : base(provider)
        {
            this.type = type;
        }

        public override object Create(Queue<ServiceTypeReference> types) => CreateInstance(types, type);
    }
}
