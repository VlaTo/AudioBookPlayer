using System;

namespace LibraProgramming.Xamarin.Dependency.Container
{
    interface IServiceLocator
    {
        object GetInstance(Type serviceType, string key = null);

        TService GetInstance<TService>(string key = null) where TService : class;
        
        object CreateInstance(Type serviceType);
        
        TService CreateInstance<TService>() where TService : class;
    }
}
