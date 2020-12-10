using System;

namespace LibraProgramming.Xamarin.Dependency.Container.Attributes
{
    [AttributeUsage(AttributeTargets.Constructor, AllowMultiple = false, Inherited = true)]
    public class PrefferedConstructorAttribute : Attribute
    {
    }
}
