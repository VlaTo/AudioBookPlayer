using System;
using System.Collections.Specialized;
using Xamarin.Forms;

namespace LibraProgramming.Xamarin.Interactivity
{
    public abstract class AttachableBindableObjectCollection<T> : BindableObjectCollection<T>
        where T : BindableObject
    {
        public BindableObject AttachedObject
        {
            get;
            protected set;
        }

        protected AttachableBindableObjectCollection()
        {
            CollectionChanged += DoCollectionChanged;
        }

        public virtual void Attach(BindableObject bindable)
        {
            if (ReferenceEquals(bindable, AttachedObject))
            {
                return;
            }

            if (false == ReferenceEquals(AttachedObject, null))
            {
                throw new Exception();
            }

            DoAttach(bindable);
        }

        public virtual void Detach()
        {
            if (ReferenceEquals(null, AttachedObject))
            {
                return;
            }

            DoDetach();
        }

        internal abstract void DoItemAdded(T bindable);

        internal abstract void DoItemRemoved(T bindable);

        //protected abstract void OnAttached();

        //protected abstract void OnDetached();

        protected abstract void DoAttach(BindableObject bindable);

        protected abstract void DoDetach();

        private void DoCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                {
                    foreach(T item in e.NewItems)
                    {
                        DoItemAdded(item);
                    }

                    break;
                }

                case NotifyCollectionChangedAction.Remove:
                {
                    foreach (T item in e.OldItems)
                    {
                        DoItemRemoved(item);
                    }

                    break;
                }

                case NotifyCollectionChangedAction.Replace:
                {
                    foreach (T item in e.OldItems)
                    {
                        DoItemRemoved(item);
                    }

                    foreach (T item in e.NewItems)
                    {
                        DoItemAdded(item);
                    }

                    break;
                }

                case NotifyCollectionChangedAction.Reset:
                {
                    foreach (T item in this)
                    {
                        DoItemRemoved(item);
                    }

                    foreach (T item in this)
                    {
                        DoItemAdded(item);
                    }

                    break;
                }
            }
        }
    }
}
