using System;
using System.Collections.Generic;

namespace LibraProgramming.Xamarin.Dependency.Container.Factories
{
    internal sealed class CreatorFactory<TService> : Factory
    {
        private readonly Func<TService> creator;
        
        public CreatorFactory(IInstanceProvider provider, Func<TService> creator)
            : base(provider)
        {
            this.creator = creator;
        }

        public override object Create(Queue<ServiceTypeReference> types) => creator.Invoke();
    }
}
