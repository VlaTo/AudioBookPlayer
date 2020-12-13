using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace LibraProgramming.Xamarin.Interactivity
{
    public class BindableObjectList : CollectionBase, IList<BindableObject>
    {
        public BindableObject this[int index]
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        public bool IsReadOnly { get; } = false;

        public void Add(BindableObject item)
        {
            if (null == item)
            {
                throw new ArgumentNullException(nameof(item));
            }

            InnerList.Add(item);
        }

        public bool Contains(BindableObject item)
        {
            if (null == item)
            {
                throw new ArgumentNullException(nameof(item));
            }

            throw new NotImplementedException();
        }

        public void CopyTo(BindableObject[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public int IndexOf(BindableObject item)
        {
            throw new NotImplementedException();
        }

        public void Insert(int index, BindableObject item)
        {
            throw new NotImplementedException();
        }

        public bool Remove(BindableObject item)
        {
            throw new NotImplementedException();
        }

        IEnumerator<BindableObject> IEnumerable<BindableObject>.GetEnumerator()
        {
            foreach (BindableObject current in InnerList)
            {
                yield return current;
            }
        }
    }
}
