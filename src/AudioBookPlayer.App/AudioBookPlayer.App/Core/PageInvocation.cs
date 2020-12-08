using System;
using Xamarin.Forms;

namespace AudioBookPlayer.App.Core
{
    internal static class PageInvocation
    {
        public static void InvokeViewModelAction<T>(BindableObject bindable, Action<T> action)
        {
            if (bindable.BindingContext is T context && null != context)
            {
                action.Invoke(context);
            }
        }
    }
}
