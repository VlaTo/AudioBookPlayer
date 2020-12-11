using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace LibraProgramming.Xamarin.Interactivity
{
    public static class Interaction
    {
        public static readonly BindableProperty InteractionRequestsProperty;

        static Interaction()
        {
            InteractionRequestsProperty = BindableProperty.CreateAttached(
                "InteractionRequests",
                typeof(InteractionRequestsCollection),
                typeof(Interaction),
                propertyChanged: OnInteractionRequestsPropertyChanged
            );
        }
    }
}
