using System;
using Xamarin.Forms;

namespace LibraProgramming.Xamarin.Interactivity
{
    public class InteractivityBase : BindableObject, IAttachableObject
    {
        internal BindableObject AttachedObject
        {
            get;
            private set;
        }

        BindableObject IAttachableObject.AttachedObject => AttachedObject;

        public virtual void Attach(BindableObject bindable)
        {
            if (null == bindable)
            {
                return;
            }

            bindable.BindingContextChanged += OnAttachedObjectBindingContextChanged;

            if (null != bindable.BindingContext)
            {
                DoBindingContextChanged(bindable, EventArgs.Empty);
            }

            AttachedObject = bindable;

            OnAttached();
        }

        public virtual void Detach()
        {
            if (null != AttachedObject)
            {
                AttachedObject.BindingContextChanged -= OnAttachedObjectBindingContextChanged;

                AttachedObject = null;

                OnDetached();
            }
        }

        protected virtual void OnAttached()
        {
        }

        protected virtual void OnDetached()
        {
        }

        protected virtual void OnBindingContextChanged(object oldValue, object newValue)
        {
        }

        private void DoBindingContextChanged(BindableObject bindable, EventArgs _)
        {
            if (null != bindable)
            {
                var bindingContext = BindingContext;

                SetBinding(
                    BindableObject.BindingContextProperty,
                    new Binding
                    {
                        Path = nameof(bindable.BindingContext),
                        Source = bindable
                    });

                OnBindingContextChanged(bindingContext, BindingContext);
            }
        }

        private void OnAttachedObjectBindingContextChanged(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
