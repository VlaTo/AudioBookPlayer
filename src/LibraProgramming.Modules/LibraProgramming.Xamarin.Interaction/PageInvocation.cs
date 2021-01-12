using System;
using System.Threading.Tasks;
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
        
        public static async Task InvokeViewModelActionAsync<T>(BindableObject bindable, Func<T, Task> action)
        {
            if (bindable is T target && null != target)
            {
                await action.Invoke(target);
            }

            if (bindable.BindingContext is T context && null != context)
            {
                await action.Invoke(context);
            }
        }
    }
}
