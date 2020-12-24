using System;
using Xamarin.Forms;

namespace LibraProgramming.Xamarin.Interaction
{
    public static class PageInvocation
    {
        public static void InvokeViewModelAction<T>(BindableObject bindable, Action<T> action)
        {
            if (bindable is T target && null != target)
            {
                action.Invoke(target);
            }

            if (bindable.BindingContext is T context && null != context)
            {
                action.Invoke(context);
            }
        }
    }
}
