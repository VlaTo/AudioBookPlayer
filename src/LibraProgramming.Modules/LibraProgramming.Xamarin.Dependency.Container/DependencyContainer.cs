using LibraProgramming.Xamarin.Dependency.Container.Factories;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace LibraProgramming.Xamarin.Dependency.Container
{
    public sealed partial class DependencyContainer : IServiceProvider, IServiceLocator, IServiceRegistry, IInstanceProvider
    {
        private readonly object gate;
        private readonly Dictionary<Type, InstanceCollection> registration;

        public DependencyContainer()
        {
            gate = new object();
            registration = new Dictionary<Type, InstanceCollection>();
        }

        public object GetService(Type serviceType) => GetInstance(serviceType);

        public object GetInstance(Type serviceType, string key = null)
        {
            if (null == serviceType)
            {
                Throw.ArgumentNull(nameof(serviceType));
            }

            var queue = new Queue<ServiceTypeReference>();

            return GetInstanceInternal(queue, serviceType, key);
        }

        public object GetInstance(
            Queue<ServiceTypeReference> queue,
            Type serviceType,
            string key)
            => GetInstanceInternal(queue, serviceType, key);

        public TService GetInstance<TService>(string key = null)
            where TService : class
             => (TService)GetInstance(typeof(TService), key);

        public TService CreateInstance<TService>()
            where TService : class
            => (TService)CreateInstance(typeof(TService));

        public object CreateInstance(Type serviceType)
        {
            if (null == serviceType)
            {
                Throw.ArgumentNull(nameof(serviceType));
            }

            lock (gate)
            {
                var factory = new TypeFactory(this, serviceType);
                var manager = InstanceLifetime.CreateNew.Invoke(factory);
                var queue = new Queue<ServiceTypeReference>();

                return manager.ResolveInstance(queue);
            }
        }

        public void Register(
            Type service,
            Func<Factory, InstanceLifetime> lifetime = null,
            string key = null,
            bool createImmediate = false)
        {
            if (null == service)
            {
                Throw.ArgumentNull(nameof(service));
            }

            var typeInfo = service.GetTypeInfo();

            if (typeInfo.IsAbstract || typeInfo.IsInterface)
            {
                Throw.UnsupportedServiceType(service);
            }

            RegisterService(service, new TypeFactory(this, service), lifetime, key, createImmediate);
        }

        public void Register(
            Type service,
            Type impl,
            Func<Factory, InstanceLifetime> lifetime = null,
            string key = null,
            bool createImmediate = false)
        {
            if (null == service)
            {
                Throw.ArgumentNull(nameof(service));
            }

            var typeInfo = service.GetTypeInfo();

            if (!typeInfo.IsAbstract && !typeInfo.IsInterface)
            {
                Throw.UnsupportedServiceType(service);
            }

            if (null == impl)
            {
                Throw.ArgumentNull(nameof(impl));
            }

            typeInfo = impl.GetTypeInfo();

            if (typeInfo.IsAbstract || typeInfo.IsInterface)
            {
                Throw.UnsupportedServiceType(impl);
            }

            RegisterService(service, new TypeFactory(this, impl), lifetime, key, createImmediate);
        }

        public void Register<TService>(
            Func<TService> factory,
            Func<Factory, InstanceLifetime> lifetime = null,
            string key = null,
            bool createImmediate = false)
            => RegisterService(typeof(TService), new CreatorFactory<TService>(this, factory), lifetime, key, createImmediate);

        public void Register<TService, TConcrete>(
            Func<Factory, InstanceLifetime> lifetime = null,
            string key = null, 
            bool createimmediate = false)
            where TConcrete : class, TService
        {
            Register(typeof(TService), typeof(TConcrete), lifetime, key, createimmediate);
        }

        public void Register<TService>(
            Func<Factory, InstanceLifetime> lifetime = null,
            string key = null,
            bool createImmediate = false)
            => Register(typeof(TService), lifetime, key, createImmediate);

        private object GetInstanceInternal(Queue<ServiceTypeReference> queue, Type serviceType, string key = null)
        {
            lock (gate)
            {
                if (!registration.TryGetValue(serviceType, out var collection))
                {
                    Throw.MissingServiceRegistration(serviceType, nameof(serviceType));
                }

                return collection[key].ResolveInstance(queue);
            }
        }
        
        private void RegisterService(
            Type service,
            Factory factory,
            Func<Factory, InstanceLifetime> lifetime,
            string key,
            bool createImmediate)
        {
            lock (gate)
            {
                if (false == registration.TryGetValue(service, out var collection))
                {
                    collection = new InstanceCollection();
                    registration.Add(service, collection);
                }
                else if (null == key)
                {
                    Throw.MissingServiceRegistration(service, nameof(service));
                }

                var manager = lifetime ?? InstanceLifetime.Singleton;

                collection[key] = manager.Invoke(factory);

                if (createImmediate)
                {
                    var queue = new Queue<ServiceTypeReference>();
                    collection[key].ResolveInstance(queue);
                }
            }
        }
    }
}
