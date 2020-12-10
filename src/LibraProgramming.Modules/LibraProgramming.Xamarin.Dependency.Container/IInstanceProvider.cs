using System;
using System.Collections.Generic;

namespace LibraProgramming.Xamarin.Dependency.Container
{
    public interface IInstanceProvider
    {
        object GetInstance(Queue<ServiceTypeReference> queue, Type serviceType, string key);
    }
}
