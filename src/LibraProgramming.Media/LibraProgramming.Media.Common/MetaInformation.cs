using System;
using System.Collections;
using System.Collections.Generic;

namespace LibraProgramming.Media.Common
{
    public sealed class MetaItemCollection : ICollection<MetaItemValue>
    {
        private readonly ArrayList items;
        private readonly List<Enumerator> enumerators;

        public int Count => items.Count;
        public bool IsReadOnly => items.IsReadOnly;

        public MetaItemValue this[int index]
        {
            get => (MetaItemValue) items[index];
            set
            {
                items[index] = value;
                DoNotify();
            }
        }

        public MetaItemCollection()
        {
            items = new ArrayList();
            enumerators = new List<Enumerator>();
        }

        public IEnumerator<MetaItemValue> GetEnumerator() => new Enumerator(this);

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public void Add(MetaItemValue value)
        {
            if (null == value)
            {
                throw new ArgumentNullException(nameof(value));
            }

            if (items.Contains(value))
            {
                return ;
            }

            items.Add(value);

            DoNotify();
        }

        public void Clear()
        {
            items.Clear();
            DoNotify();
        }

        public bool Contains(MetaItemValue item)
        {
            if (null == item)
            {
                throw new ArgumentNullException(nameof(item));
            }

            return items.Contains(item);
        }

        public void CopyTo(MetaItemValue[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public bool Remove(MetaItemValue item)
        {
            if (null == item)
            {
                throw new ArgumentNullException(nameof(item));
            }

            var index = items.IndexOf(item);

            if (0 > index)
            {
                return false;
            }

            items.RemoveAt(index);

            DoNotify();

            return true;
        }

        private IDisposable Subscribe(Enumerator enumerator)
        {
            enumerators.Add(enumerator);
            return new Subscription(this, enumerator);
        }

        private void DoNotify()
        {
            foreach (var enumerator in enumerators)
            {
                enumerator.CollectionInvalidated();
            }
        }

        private void RemoveEnumerator(Enumerator enumerator)
        {
            if (enumerators.Remove(enumerator))
            {
                ;
            }
        }

        private sealed class Enumerator : IEnumerator<MetaItemValue>
        {
            private MetaItemCollection collection;
            private IDisposable subscription;
            private bool collectionInvalidated;
            private bool disposed;
            private int index;

            public MetaItemValue Current
            {
                get;
                private set;
            }

            object IEnumerator.Current => Current;

            public Enumerator(MetaItemCollection collection)
            {
                this.collection = collection;

                index = -1;
                disposed = false;
                collectionInvalidated = false;
                subscription = collection.Subscribe(this);
            }

            public bool MoveNext()
            {
                EnsureNoDisposed();
                EnsureNoInvalidated();

                if (0 > index)
                {
                    if (1 > collection.Count)
                    {
                        Current = null;
                        return false;
                    }

                    index = 0;
                    Current = collection[index];

                    return true;
                }

                if (collection.Count > index)
                {
                    if (collection.Count == ++index)
                    {
                        Current = null;
                        return false;
                    }

                    Current = collection[index];

                    return true;
                }

                return false;
            }

            public void Reset()
            {
                EnsureNoDisposed();
                EnsureNoInvalidated();

                index = -1;
                Current = null;
            }

            public void Dispose()
            {
                if (disposed)
                {
                    return;
                }

                Dispose(true);
            }

            public void CollectionInvalidated()
            {
                collectionInvalidated = true;
            }

            private void EnsureNoDisposed()
            {
                if (disposed)
                {
                    throw new ObjectDisposedException(nameof(Enumerator));
                }
            }

            private void EnsureNoInvalidated()
            {
                if (collectionInvalidated)
                {
                    throw new InvalidOperationException();
                }
            }

            private void Dispose(bool dispose)
            {
                if (disposed)
                {
                    return;
                }

                try
                {
                    if (dispose)
                    {
                        subscription.Dispose();
                        subscription = null;
                        collection = null;
                    }
                }
                finally
                {
                    disposed = true;
                }
            }
        }

        private sealed class Subscription : IDisposable
        {
            private MetaItemCollection collection;
            private Enumerator enumerator;
            private bool disposed;

            public Subscription(MetaItemCollection collection, Enumerator enumerator)
            {
                this.collection = collection;
                this.enumerator = enumerator;
            }

            public void Dispose()
            {
                if (disposed)
                {
                    return;
                }

                Dispose(true);
            }

            private void Dispose(bool dispose)
            {
                if (disposed)
                {
                    return;
                }

                try
                {
                    if (dispose)
                    {
                        collection.RemoveEnumerator(enumerator);
                        collection = null;
                        enumerator = null;
                    }
                }
                finally
                {
                    disposed = true;
                }
            }
        }
    }

    public sealed class MetaInformation : IEnumerable<KeyValuePair<string, MetaItemCollection>>
    {
        private readonly Dictionary<string, MetaItemCollection> items;

        public MetaItemCollection this[string key] => items.TryGetValue(key, out var collection) ? collection : null;

        public MetaInformation()
        {
            items = new Dictionary<string, MetaItemCollection>();
        }

        public void Add(string key, MetaItemValue item)
        {
            if (null == item)
            {
                throw new ArgumentNullException(nameof(item));
            }

            var collection = GetOdAddCollection(key);

            collection.Add(item);
        }

        public IEnumerator<KeyValuePair<string, MetaItemCollection>> GetEnumerator() => items.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        private MetaItemCollection GetOdAddCollection(string key)
        {
            if (false == items.TryGetValue(key, out var collection))
            {
                collection = new MetaItemCollection();
                items.Add(key, collection);
            }

            return collection;
        }


    }
}
