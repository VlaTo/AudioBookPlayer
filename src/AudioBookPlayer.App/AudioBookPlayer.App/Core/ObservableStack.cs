using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;

namespace AudioBookPlayer.App.Core
{
    public sealed class ObservableStack<T> : ICollection, IReadOnlyCollection<T>, INotifyCollectionChanged, INotifyPropertyChanged
    {
        private readonly ArrayList items;
        private readonly WeakEventManager eventManager;

        public int Count => items.Count;

        public bool IsSynchronized => items.IsSynchronized;

        public object SyncRoot => items.SyncRoot;

        public bool IsReadOnly => throw new NotImplementedException();

        public event NotifyCollectionChangedEventHandler CollectionChanged
        {
            add => eventManager.AddEventHandler(value);
            remove => eventManager.RemoveEventHandler(value);
        }

        public event PropertyChangedEventHandler PropertyChanged
        {
            add => eventManager.AddEventHandler(value);
            remove => eventManager.RemoveEventHandler(value);
        }

        public ObservableStack()
        {
            items = new ArrayList();
            eventManager = new WeakEventManager();
        }

        public bool Contains(T item) => items.Contains(item);

        public void Push(T item)
        {
            if (null == item)
            {
                throw new ArgumentNullException(nameof(item));
            }

            items.Add(item);

            RaisePushEvent(item, items.Count - 1);
        }

        public T Pop()
        {
            if (0 == items.Count)
            {
                throw new InvalidOperationException();
            }

            var last = items.Count - 1;
            var item = (T)items[last];

            items.RemoveAt(last);
            RaisePopEvent(item, last);

            return item;
        }

        public T Peek()
        {
            if (0 == items.Count)
            {
                return default;
            }

            var last = items.Count - 1;

            return (T)items[last];
        }

        public IEnumerator<T> GetEnumerator() => new CollectionEnumerator(this);

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public void CopyTo(Array array, int index) => items.CopyTo(array, index);

        private void RaisePushEvent(T item, int index)
        {
            var args = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, index);
            eventManager.RaiseEvent(this, args, nameof(CollectionChanged));
            eventManager.RaiseEvent(this, new PropertyChangedEventArgs(nameof(Count)), nameof(PropertyChanged));
        }

        private void RaisePopEvent(T item, int index)
        {
            var args = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item, index);
            eventManager.RaiseEvent(this, args, nameof(CollectionChanged));
            eventManager.RaiseEvent(this, new PropertyChangedEventArgs(nameof(Count)), nameof(PropertyChanged));
        }

        /// <summary>
        /// 
        /// </summary>
        private sealed class StackEnumerator : IEnumerator<T>
        {
            private ObservableStack<T> stack;
            private bool disposed;
            private bool modified;
            private int? index;

            public T Current
            {
                get
                {
                    EnsureNotDisposed();
                    EnsureNoModified();

                    if (false == index.HasValue)
                    {
                        throw new Exception();
                    }

                    return (T)stack.items[index.Value];
                }
            }

            object IEnumerator.Current => Current;

            public StackEnumerator(ObservableStack<T> stack)
            {
                this.stack = stack;
                index = null;
                stack.PropertyChanged += OnStackPropertyChanged;
            }

            public void Dispose()
            {
                Dispose(true);
            }

            public bool MoveNext()
            {
                if (false == index.HasValue)
                {
                    var last = stack.items.Count - 1;

                    if (-1 < last)
                    {
                        index = last;
                        return true;
                    }

                    return false;
                }

                index--;

                return 0 <= index;
            }

            public void Reset()
            {
                EnsureNotDisposed();
                
                modified = false;
                index = null;
            }

            private void EnsureNoModified()
            {
                if (modified)
                {
                    throw new Exception();
                }
            }

            private void EnsureNotDisposed()
            {
                if (disposed)
                {
                    throw new ObjectDisposedException(GetType().Name);
                }
            }

            private void OnStackPropertyChanged(object sender, PropertyChangedEventArgs e) => modified = true;

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
                        stack.PropertyChanged -= OnStackPropertyChanged;
                        stack = null;
                    }
                }
                finally
                {
                    disposed = true;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private sealed class CollectionEnumerator : IEnumerator<T>
        {
            private ObservableStack<T> stack;
            private bool disposed;
            private bool modified;
            private int? index;

            public T Current
            {
                get
                {
                    EnsureNotDisposed();
                    EnsureNoModified();

                    if (false == index.HasValue)
                    {
                        throw new Exception();
                    }

                    return (T)stack.items[index.Value];
                }
            }

            object IEnumerator.Current => Current;

            public CollectionEnumerator(ObservableStack<T> stack)
            {
                this.stack = stack;
                index = null;
                stack.PropertyChanged += OnStackPropertyChanged;
            }

            public void Dispose()
            {
                Dispose(true);
            }

            public bool MoveNext()
            {
                if (false == index.HasValue)
                {
                    var first = 0;

                    if (stack.items.Count > first)
                    {
                        index = first;
                        return true;
                    }

                    return false;
                }

                index++;

                return stack.items.Count <= index;
            }

            public void Reset()
            {
                EnsureNotDisposed();

                modified = false;
                index = null;
            }

            private void EnsureNoModified()
            {
                if (modified)
                {
                    throw new Exception();
                }
            }

            private void EnsureNotDisposed()
            {
                if (disposed)
                {
                    throw new ObjectDisposedException(GetType().Name);
                }
            }

            private void OnStackPropertyChanged(object sender, PropertyChangedEventArgs e) => modified = true;

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
                        stack.PropertyChanged -= OnStackPropertyChanged;
                        stack = null;
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
