using System.Collections.Generic;

namespace LibraProgramming.Xamarin.Dependency.Container
{
    public abstract partial class InstanceLifetime
    {
        protected Factory Factory
        {
            get;
        }

        public abstract object ResolveInstance(Queue<ServiceTypeReference> queue);

        protected InstanceLifetime(Factory factory)
        {
            Factory = factory;
        }
    }
}
