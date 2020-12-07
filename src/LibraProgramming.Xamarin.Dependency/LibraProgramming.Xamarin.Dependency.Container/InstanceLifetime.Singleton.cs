using System;
using System.Collections.Generic;

namespace LibraProgramming.Xamarin.Dependency.Container
{
    public partial class InstanceLifetime
    {
        public static Func<Factory, InstanceLifetime> Singleton => factory => new SingletonLifetime(factory);

        /// <summary>
        /// 
        /// </summary>
        public class SingletonLifetime : InstanceLifetime
        {
            private object instance;

            public SingletonLifetime(Factory factory)
                : base(factory)
            {
            }

            public override object ResolveInstance(Queue<ServiceTypeReference> queue) =>
                instance ?? (instance = Factory.Create(queue));
        }
    }
}
