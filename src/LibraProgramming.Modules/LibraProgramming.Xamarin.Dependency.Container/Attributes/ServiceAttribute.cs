using System;

namespace LibraProgramming.Xamarin.Dependency.Container.Attributes
{
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = false)]
    public sealed class ServiceAttribute : Attribute
    {
        public string Key
        {
            get;
        }

        public ServiceAttribute(string key)
        {
            Key = key;
        }
    }
}
