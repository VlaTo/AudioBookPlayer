using System;
using Xamarin.Forms;

namespace LibraProgramming.Xamarin.Interactivity
{
    public static class Interaction
    {
        public static readonly BindableProperty RequestProperty;

        static Interaction()
        {
            RequestProperty = BindableProperty.CreateAttached(
                "Request",
                typeof(InteractionRequestCollection),
                typeof(Interaction),
                null,
                defaultBindingMode: BindingMode.OneWay,
                propertyChanged: OnRequestPropertyChanged
            );
        }

        private static void OnRequestPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            throw new NotImplementedException();
        }
    }
}
