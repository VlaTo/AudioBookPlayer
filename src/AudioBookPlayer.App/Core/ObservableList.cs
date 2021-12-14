using System;
using System.Collections;
using System.Collections.Generic;

namespace AudioBookPlayer.App.Core
{
    internal class ObservableList<T> : IList<T>, IObservableList<T>
    {
        private readonly ArrayList list;
        private readonly List<Subscription> subscriptions;

        public int Count => list.Count;

        public bool IsReadOnly { get; } = false;

        public T this[int index]
        {
            get
            {
                if (0 > index || index >= list.Count)
                {
                    throw new IndexOutOfRangeException();
                }

                return (T)list[index];
            }
            set
            {
                if (0 > index || index > list.Count)
                {
                    throw new IndexOutOfRangeException();
                }

                list[index] = value;

                var temp = subscriptions.ToArray();

                for (var position = 0; position < temp.Length; position++)
                {
                    var observer = temp[position].Observer;
                    observer.OnReplace(index, value);
                }
            }
        }

        public ObservableList()
        {
            list = new ArrayList();
            subscriptions = new List<Subscription>();
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public IDisposable Subscribe(IListObserver<T> observer)
        {
            var subscription = subscriptions.Find(x => ReferenceEquals(x.Observer, observer));

            if (null != subscription)
            {
                return subscription;
            }

            subscription = new Subscription(this, observer);
            subscriptions.Add(subscription);

            return subscription;
        }

        public int IndexOf(T item) => list.IndexOf(item);

        public void Insert(int index, T item)
        {
            if (0 > index || list.Count < index)
            {
                throw new IndexOutOfRangeException();
            }

            list.Insert(index, item);

            var temp = subscriptions.ToArray();

            for (var position = 0; position < temp.Length; position++)
            {
                var observer = temp[position].Observer;
                observer.OnAdded(index, item);
            }
        }

        public void RemoveAt(int index)
        {
            if (0 > index || list.Count <= index)
            {
                throw new IndexOutOfRangeException();
            }

            list.RemoveAt(index);

            var temp = subscriptions.ToArray();

            for (var position = 0; position < temp.Length; position++)
            {
                var observer = temp[position].Observer;
                observer.OnRemove(index);
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            for (var index = 0; index < list.Count; index++)
            {
                yield return (T)list[index];
            }
        }

        public void Add(T item)
        {
            var position = list.Add(item);
            var temp = subscriptions.ToArray();

            for (var index = 0; index < temp.Length; index++)
            {
                var observer = temp[index].Observer;
                observer.OnAdded(position, item);
            }
        }

        public void Clear()
        {
            list.Clear();

            var temp = subscriptions.ToArray();

            for (var position = 0; position < temp.Length; position++)
            {
                var observer = temp[position].Observer;
                observer.OnClear();
            }
        }

        public bool Contains(T item) => list.Contains(item);

        public void CopyTo(T[] array, int arrayIndex) => list.CopyTo(array, arrayIndex);

        public bool Remove(T item)
        {
            var position = list.IndexOf(item);

            if (0 > position)
            {
                return false;
            }

            list.RemoveAt(position);

            var temp = subscriptions.ToArray();

            for (var index = 0; index < temp.Length; index++)
            {
                var observer = temp[index].Observer;
                observer.OnRemove(position);
            }

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        private sealed  class Subscription : IDisposable
        {
            private readonly ObservableList<T> owner;

            public IListObserver<T> Observer
            {
                get; 
            }

            public Subscription(ObservableList<T> owner, IListObserver<T> observer)
            {
                Observer = observer;
                this.owner = owner;
            }

            public void Dispose()
            {
                throw new NotImplementedException();
            }
        }
    }
}