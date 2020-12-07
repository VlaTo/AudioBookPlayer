using System;
using System.Collections;
using System.Collections.Generic;

namespace LibraProgramming.Xamarin.Dependency.Container
{
    public partial class DependencyContainer
    {
        private class InstanceCollection : IEnumerable<InstanceLifetime>
        {
            private readonly Dictionary<string, InstanceLifetime> instances;

            /// <summary>
            /// Initializes a new instance of the <see cref="T:System.Object"/> class.
            /// </summary>
            public InstanceCollection()
            {
                instances = new Dictionary<string, InstanceLifetime>();
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="key"></param>
            /// <returns></returns>
            public InstanceLifetime this[string key]
            {
                get => instances[key ?? String.Empty];
                set => instances[key ?? String.Empty] = value;
            }

            /// <inheritdoc cref="IEnumerable.GetEnumerator" />
            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

            /// <inheritdoc cref="IEnumerable{T}.GetEnumerator" />
            public IEnumerator<InstanceLifetime> GetEnumerator()
            {
                var keys = instances.Keys;

                foreach (var key in keys)
                {
                    yield return instances[key];
                }
            }
        }
    }
}
