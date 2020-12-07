using AudioBookPlayer.App.Core.Attributes;
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
            }
        }
    }
}
