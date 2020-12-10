using AudioBookPlayer.App.Core.Attributes;
using LibraProgramming.Xamarin.Interactivity.Behaviors;
using System;
using Xamarin.Forms;

namespace AudioBookPlayer.App.Core
{
    internal sealed class ViewModelProvider
    {
        public static void AutoWireViewModelChanged(BindableObject bindable, Action<BindableObject, object> bind)
        {
            var attribute = (ViewModelAttribute)Attribute.GetCustomAttribute(bindable.GetType(), typeof(ViewModelAttribute));

            if (null != attribute)
            {
                var container = AudioBookPlayerApp.Current.DependencyContainer;
                var viewModel = container.CreateInstance(attribute.Type);

                bind.Invoke(bindable, viewModel);

                if (bindable is Page page)
                {
                    foreach (var behavior in page.Behaviors)
                    {
                        if (behavior is PageLifecycleBehavior)
                        {
                            return;
                        }
                    }

                    page.Behaviors.Add(new PageLifecycleBehavior());
                }
            }
        }

        /*private static void BindInitialize(BindableObject bindable)
        {
            if (bindable is Page page)
            {
                page.Behaviors.Add(new PageLifecycleBehavior());
            }
        }

        private static void DoAppearing(object sender, EventArgs e)
        {
            var page = (Page)sender;

            page.Appearing -= DoAppearing;

            if (page.BindingContext is IInitialize model)
            {
                model.OnInitialize();
            }
        }

        private static void DoDisappearing(object sender, EventArgs e)
        {
            var page = (Page)sender;

            page.Disappearing -= DoDisappearing;

            if (page.BindingContext is IDestructible model)
            {
                model.OnInitialize();
            }
        }*/
    }
}
