using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace AudioBookPlayer.Domain
{
    public sealed class ArrayList<T> : IList<T>, IReadOnlyList<T>
    {
        private int size;
        private T[] items;
        private int version;

        public int Count => size;

        public bool IsReadOnly { get; } = true;

        public int Capacity
        {
            get => items.Length;
            set
            {
                if (value < size)
                {
                    throw new ArgumentOutOfRangeException(nameof(value));
                }

                if (value == items.Length)
                {
                    return;
                }

                if (0 < value)
                {
                    var destination = new T[value];
                    
                    if (0 < size)
                    {
                        Array.Copy(items, 0, destination, 0, size);
                    }

                    items = destination;
                }
                else
                {
                    items = new T[4];
                }
            }
        }

        public T this[int index]
        {
            get
            {
                if (0 <= index && index < size)
                {
                    return items[index];
                }

                throw new ArgumentOutOfRangeException(nameof(index));
            }
            set
            {
                if (0 > index || index >= size)
                {
                    throw new ArgumentOutOfRangeException(nameof(index));
                }

                items[index] = value;

                ++version;
            }
        }

        public ArrayList()
        {
            items = Array.Empty<T>();
            size = 0;
        }

        public ArrayList(int capacity)
        {
            if (0 > capacity)
            {
                throw new ArgumentOutOfRangeException(nameof(capacity));
            }

            size = 0;
            items = (0 == capacity) ? Array.Empty<T>() : new T[capacity];
        }

        public IEnumerator<T> GetEnumerator() => new ArrayListEnumerator(this);

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public void Add(T item)
        {
            if (size == items.Length)
            {
                EnsureCapacity(size + 1);
            }

            items[size] = item;

            ++version;
            size++;
        }

        public void Clear()
        {
            if (0 < size)
            {
                Array.Clear(items, 0, size);
                size = 0;
            }

            ++version;
        }

        public bool Contains(T item)
        {
            if (null == item)
            {
                for (var index = 0; index < size; index++)
                {
                    if (null == items[index])
                    {
                        return true;
                    }
                }

                return false;
            }

            var comparer = EqualityComparer<T>.Default;

            for (var index = 0; index < size; index++)
            {
                if (null != items[index] && comparer.Equals(items[index], item))
                {
                    return true;
                }
            }

            return false;
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            if (null != array && 1 != array.Rank)
            {
                throw new ArgumentException("", nameof(array));
            }

            Array.Copy(items, 0, array, arrayIndex, size);
        }

        public bool Remove(T item)
        {
            var index = IndexOf(item);

            if (0 > index)
            {
                return false;
            }

            RemoveAt(index);

            return true;
        }

        public int IndexOf(T item) => Array.IndexOf(items, item, 0, size);
        
        public int IndexOf(T item, int startIndex)
        {
            if (size < startIndex)
            {
                throw new ArgumentOutOfRangeException(nameof(startIndex));
            }

            return Array.IndexOf(items, item, startIndex, size - startIndex);
        }

        public int IndexOf(T value, int startIndex, int count)
        {
            if (size < startIndex)
            {
                throw new ArgumentOutOfRangeException(nameof(startIndex));
            }

            if (0 > count || startIndex > (size - count))
            {
                throw new ArgumentOutOfRangeException(nameof(count));
            }

            return Array.IndexOf(items, value, startIndex, count);
        }

        public void Insert(int index, T item)
        {
            if (0 > index || index > size)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            if (items.Length == size)
            {
                EnsureCapacity(size + 1);
            }

            if (size > index)
            {
                Array.Copy(items, index, items, index + 1, size - index);
            }

            items[index] = item;

            ++version;
            size++;
        }

        public void RemoveAt(int index)
        {
            if (0 > index || index >= size)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            --size;

            if (size > index)
            {
                Array.Copy(items, index + 1, items, index, size - index);
            }

            items[size] = default;

            ++version;
        }


        private void EnsureCapacity(int min)
        {
            if (min <= items.Length)
            {
                return;
            }

            var num = items.Length == 0 ? 4 : items.Length << 1;

            if ((uint)num > 2146435071U)
            {
                num = 2146435071;
            }

            if (num < min)
            {
                num = min;
            }

            Capacity = num;
        }

        private sealed class ArrayListEnumerator : IEnumerator<T>
        {
            private ArrayList<T> list;
            private int version;
            private int index;
            private T current;
            private bool disposed;

            public T Current
            {
                get
                {
                    if (-1 == index || index >= list.size)
                    {
                        throw new InvalidOperationException("");
                    }

                    return current;
                }
            }

            object IEnumerator.Current => Current;

            public ArrayListEnumerator(ArrayList<T> list)
            {
                this.list = list;

                index = -1;
                version = list.version;
                current = default;
            }

            public bool MoveNext()
            {
                EnsureNotModified();

                if ((list.size - 1) > index)
                {
                    current = list.items[++index];
                    return true;
                }

                current = default;
                index = list.size;

                return false;
            }

            public void Reset()
            {
                EnsureNotModified();

                current = default;
                index = -1;
            }


            public void Dispose()
            {
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
                        list = null;
                        version = -1;
                        index = -1;
                        current = default;
                    }
                }
                finally
                {
                    disposed = true;
                }
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            private void EnsureNotModified()
            {
                if (list.version != version)
                {
                    throw new InvalidOperationException("");
                }
            }
        }
    }
}