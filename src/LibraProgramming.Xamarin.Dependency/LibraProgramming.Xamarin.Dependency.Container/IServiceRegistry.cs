using System;

namespace LibraProgramming.Xamarin.Dependency.Container
{
    public interface IServiceRegistry
    {
        void Register(
            Type service,
            Func<Factory, InstanceLifetime> lifetime = null,
            string key = null,
            bool createImmediate = false
        );

        void Register(
            Type service,
            Type impl,
            Func<Factory, InstanceLifetime> lifetime = null,
            string key = null,
            bool createImmediate = false
        );

        void Register<TService>(
            Func<TService> factory,
            Func<Factory, InstanceLifetime> lifetime = null,
            string key = null,
            bool createImmediate = false
        );

        void Register<TService, TConcrete>(
            Func<Factory, InstanceLifetime> lifetime = null,
            string key = null,
            bool createimmediate = false)
            where TConcrete : class, TService;
    }
}
