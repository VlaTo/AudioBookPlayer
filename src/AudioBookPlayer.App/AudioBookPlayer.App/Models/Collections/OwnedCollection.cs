using System;
using System.Collections;
using System.Collections.Generic;

namespace AudioBookPlayer.App.Models.Collections
{
    public enum ChangeAction
    {
        Added,
        Insert,
        Changed,
        Removed,
        Clear
    }

    public delegate void CollectionChanged(ChangeAction action, int index);

    public sealed class OwnedCollection<TValue> : IList<TValue>
    {
        private readonly CollectionChanged action;
        private readonly ArrayList items;
        private long version;

        public int Count => items.Count;

        public bool IsReadOnly => items.IsReadOnly;

        public TValue this[int index]
        {
            get => (TValue) items[index];
            set
            {
                items[index] = value;
                
                version++;

                action.Invoke(ChangeAction.Changed, index);
            }
        }

        public OwnedCollection(CollectionChanged action)
        {
            this.action = action;
            items = new ArrayList();
        }

        public IEnumerator<TValue> GetEnumerator() => new Enumerator(this);

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public void Add(TValue item)
        {
            if (null == item)
            {
                throw new ArgumentNullException(nameof(item));
            }

            version++;

            var index = items.Add(item);

            action.Invoke(ChangeAction.Added, index);
        }

        public void Clear()
        {
            if (0 == items.Count)
            {
                return;
            }

            version++;

            items.Clear();

            action.Invoke(ChangeAction.Clear, -1);
        }

        public bool Contains(TValue item) => items.Contains(item);

        public void CopyTo(TValue[] array, int arrayIndex)
        {
            items.CopyTo(array, arrayIndex);
        }

        public int IndexOf(TValue item) => items.IndexOf(item);

        public void Insert(int index, TValue item)
        {
            if (null == item)
            {
                throw new ArgumentNullException(nameof(item));
            }

            version++;

            items.Insert(index, item);

            action.Invoke(ChangeAction.Insert, index);
        }

        public void RemoveAt(int index)
        {
            if (0 > index)
            {
                return ;
            }

            version++;

            items.RemoveAt(index);

            action.Invoke(ChangeAction.Removed, index);
        }

        public bool Remove(TValue item)
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

            version++;

            items.RemoveAt(index);

            action.Invoke(ChangeAction.Removed, index);

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        private sealed class Enumerator : IEnumerator<TValue>
        {
            private OwnedCollection<TValue> owner;
            private long version;
            private TValue current;
            private int index;
            private bool disposed;

            public TValue Current => current;

            object IEnumerator.Current => Current;

            public Enumerator(OwnedCollection<TValue> owner)
            {
                this.owner = owner;
                version = owner.version;
                
                Reset();
            }

            public bool MoveNext()
            {
                Ensure();

                if (0 > index)
                {
                    if (0 < owner.Count)
                    {
                        index = 0;
                        current = owner[index];

                        return true;
                    }

                    return false;
                }

                if (owner.Count > index)
                {
                    if (owner.Count == ++index)
                    {
                        current = default;
                        return false;
                    }

                    current = owner[index];

                    return true;
                }

                return false;
            }

            public void Reset()
            {
                Ensure();

                current = default;
                index = -1;
            }

            public void Dispose()
            {
                if (false == disposed)
                {
                    Dispose(true);
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
                        owner = null;
                        current = default;
                        index = -1;
                    }
                }
                finally
                {
                    disposed = true;
                }
            }

            private void Ensure()
            {
                if (disposed)
                {
                    throw new ObjectDisposedException(GetType().FullName);
                }

                if (version != owner.version)
                {
                    throw new InvalidOperationException();
                }
            }
        }
    }
}