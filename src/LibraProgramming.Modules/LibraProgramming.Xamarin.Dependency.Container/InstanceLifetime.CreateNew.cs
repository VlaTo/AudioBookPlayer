using LibraProgramming.Xamarin.Dependency.Container.Factories;
using System;
using System.Collections.Generic;

namespace LibraProgramming.Xamarin.Dependency.Container
{
    public partial class InstanceLifetime
    {
        public static Func<Factory, InstanceLifetime> CreateNew => factory => new CreateNewLifetime(factory);

        /// <summary>
        /// 
        /// </summary>
        public class CreateNewLifetime : InstanceLifetime
        {
            public CreateNewLifetime(Factory factory)
                : base(factory)
            {
            }

            public override object ResolveInstance(Queue<ServiceTypeReference> queue) => Factory.Create(queue);
        }
    }
}
