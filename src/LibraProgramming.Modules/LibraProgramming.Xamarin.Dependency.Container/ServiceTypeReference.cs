using System;

namespace LibraProgramming.Xamarin.Dependency.Container
{
    public sealed class ServiceTypeReference : IEquatable<ServiceTypeReference>
    {
        public Type Type
        {
            get;
        }
        
        public string Key
        {
            get;
        }
        
        public ServiceTypeReference(Type type, string key = null)
        {
            Type = type;
            Key = key;
        }

        public bool Equals(ServiceTypeReference other)
        {
            if (null == other)
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return Type == other.Type && String.Equals(Key, other.Key);
        }
    }
}
