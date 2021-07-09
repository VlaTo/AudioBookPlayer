using System;
using System.Collections;
using System.Collections.Generic;

namespace LibraProgramming.Media.Common
{
    public sealed class TagsCollection : ICollection<TagValue>
    {
        private readonly ArrayList items;
        private readonly List<Enumerator> enumerators;

        public int Count => items.Count;
        public bool IsReadOnly => items.IsReadOnly;

        public TagValue this[int index]
        {
            get => (TagValue) items[index];
            set
            {
                items[index] = value;
                DoNotify();
            }
        }

        public TagsCollection()
        {
            items = new ArrayList();
            enumerators = new List<Enumerator>();
        }

        public IEnumerator<TagValue> GetEnumerator() => new Enumerator(this);

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public void Add(TagValue value)
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

        public bool Contains(TagValue item)
        {
            if (null == item)
            {
                throw new ArgumentNullException(nameof(item));
            }

            return items.Contains(item);
        }

        public void CopyTo(TagValue[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public bool Remove(TagValue item)
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

        private sealed class Enumerator : IEnumerator<TagValue>
        {
            private TagsCollection collection;
            private IDisposable subscription;
            private bool collectionInvalidated;
            private bool disposed;
            private int index;

            public TagValue Current
            {
                get;
                private set;
            }

            object IEnumerator.Current => Current;

            public Enumerator(TagsCollection collection)
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
            private TagsCollection collection;
            private Enumerator enumerator;
            private bool disposed;

            public Subscription(TagsCollection collection, Enumerator enumerator)
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
}