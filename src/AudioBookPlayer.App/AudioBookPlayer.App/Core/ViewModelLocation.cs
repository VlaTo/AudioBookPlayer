using Xamarin.Forms;

namespace AudioBookPlayer.App.Core
{
    public static class ViewModelLocation
    {
        private const string AutowireViewModelPropertyName = "AutowireViewModelProperty";

        public static readonly BindableProperty AutowireViewModelProperty;

        static ViewModelLocation()
        {
            AutowireViewModelProperty = BindableProperty.CreateAttached(
                AutowireViewModelPropertyName,
                typeof(bool?),
                typeof(ViewModelLocation),
                null,
                propertyChanged: OnAutowireViewModelPropertyChanged
            );
        }

        public static bool? GetAutowireViewModel(BindableObject bindable)
        {
            return (bool?)bindable.GetValue(AutowireViewModelProperty);
        }

        public static void SetAutowireViewModel(BindableObject bindable, bool? value)
        {
            bindable.SetValue(AutowireViewModelProperty, value);
        }

        private static void OnAutowireViewModelPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            bool? bNewValue = (bool?)newValue;

            if (bNewValue.HasValue && bNewValue.Value)
            {
                ViewModelProvider.AutoWireViewModelChanged(bindable, BindViewModel);
            }
        }

        private static void BindViewModel(BindableObject view, object viewModel)
        {
            view.BindingContext = viewModel;
        }
    }
}
