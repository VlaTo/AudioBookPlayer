using System;

namespace LibraProgramming.Xamarin.Dependency.Container
{
    public sealed class DependencyContainer : IServiceProvider
    {
        public DependencyContainer()
        {

        }

        public object GetService(Type serviceType)
        {
            throw new NotImplementedException();
        }
    }
}
