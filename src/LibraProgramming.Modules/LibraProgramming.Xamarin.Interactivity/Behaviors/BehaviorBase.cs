using System;
using Xamarin.Forms;

namespace LibraProgramming.Xamarin.Interactivity.Behaviors
{
    public class BehaviorBase<T> : Behavior<T>
        where T : BindableObject
    {
        public T AttachedObject
        {
            get;
            private set;
        }

        protected override void OnAttachedTo(T bindable)
        {
            base.OnAttachedTo(bindable);

            AttachedObject = bindable;

            if (null != bindable.BindingContext)
            {
                BindingContext = bindable.BindingContext;
            }

            bindable.BindingContextChanged += DoBindingContextChanged;
        }

        protected override void OnDetachingFrom(T bindable)
        {
            base.OnDetachingFrom(bindable);

            bindable.BindingContextChanged -= DoBindingContextChanged;
        }

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();

            BindingContext = AttachedObject.BindingContext;
        }

        private void DoBindingContextChanged(object sender, EventArgs e)
        {
            OnBindingContextChanged();
        }
    }
}
