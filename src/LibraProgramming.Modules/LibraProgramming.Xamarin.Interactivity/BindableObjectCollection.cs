using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Xamarin.Forms;

namespace LibraProgramming.Xamarin.Interactivity
{
    public class BindableObjectCollection<T> : BindableObject, IList<T>, INotifyCollectionChanged
        where T : BindableObject
    {
        private readonly ObservableCollection<T> objects;

        public T this[int index]
        {
            get => objects[index];
            set => objects[index] = value;
        }

        public int Count => objects.Count;

        public bool IsReadOnly { get; } = false;

        public event NotifyCollectionChangedEventHandler CollectionChanged
        {
            add
            {
                objects.CollectionChanged += value;
            }
            remove
            {
                objects.CollectionChanged -= value;
            }
        }

        protected BindableObjectCollection()
        {
            objects = new ObservableCollection<T>();
        }

        public void Add(T item) => objects.Add(item);

        public void Clear() => objects.Clear();

        public bool Contains(T item) => objects.Contains(item);

        public void CopyTo(T[] array, int arrayIndex) => objects.CopyTo(array, arrayIndex);

        public IEnumerator<T> GetEnumerator() => objects.GetEnumerator();

        public int IndexOf(T item) => objects.IndexOf(item);

        public void Insert(int index, T item) => objects.Insert(index, item);

        public bool Remove(T item) => objects.Remove(item);

        public void RemoveAt(int index) => objects.RemoveAt(index);

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
