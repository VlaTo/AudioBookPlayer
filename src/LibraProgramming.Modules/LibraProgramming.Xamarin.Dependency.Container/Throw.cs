using System;

namespace LibraProgramming.Xamarin.Dependency.Container
{
    internal static class Throw
    {
        public static ArgumentException ArgumentNull(string paramName)
        {
            return new ArgumentNullException(paramName, "");
        }
        
        public static ArgumentException MissingServiceRegistration(Type serviceType, string paramName)
        {
            return new ArgumentException($"Service of type: \"{serviceType.FullName}\" not resolved.", paramName);
        }
        
        public static ArgumentException DuplicateServiceRegistration(Type serviceType, string paramName)
        {
            return new ArgumentException($"Service of type: \"{serviceType.FullName}\" has already registered.", paramName);
        }
        
        public static ArgumentException UnsupportedServiceType(Type serviceType)
        {
            var message = String.Format("Unsupported type:{0}", serviceType.Name);
            return new ArgumentException(message);
        }
        
        public static Exception CyclicServiceReference(Type type, string argumentName, Type serviceType, string key)
        {
            var message = String.IsNullOrEmpty(key)
                ? String.Format("Service: {0} has cyclic reference", serviceType)
                : String.Format("Service: {0} with key: {1} has cyclic reference", serviceType, key);

            //throw new ServiceLocatorException(message);
            return new Exception(message);
        }
    }
}