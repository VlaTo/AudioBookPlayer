using System;
using Xamarin.Forms;

namespace LibraProgramming.Xamarin.Interactivity
{
    public static class Interaction
    {
        public static readonly BindableProperty RequestTriggersProperty;

        static Interaction()
        {
            RequestTriggersProperty = BindableProperty.CreateAttached(
                "RequestTriggers",
                typeof(InteractionRequestCollection),
                typeof(Interaction),
                null,
                propertyChanged: OnRequestTriggersPropertyChanged,
                defaultValueCreator: CreateRequestTriggers
            );
        }

        public static InteractionRequestCollection GetRequestTriggers(BindableObject bindable)
        {
            var requests = (InteractionRequestCollection)bindable.GetValue(RequestTriggersProperty);

            if (null == requests)
            {
                requests = new InteractionRequestCollection();
                bindable.SetValue(RequestTriggersProperty, requests);
            }

            return requests;
        }

        private static object CreateRequestTriggers(BindableObject bindable)
        {
            var collection = new InteractionRequestCollection();

            OnRequestTriggersPropertyChanged(bindable, null, collection);

            return collection;
        }

        private static void OnRequestTriggersPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var previous = oldValue as InteractionRequestCollection;
            var current = newValue as InteractionRequestCollection;

            if (previous == current)
            {
                return;
            }

            if (previous != null && null != previous.AttachedObject)
            {
                previous.Detach();
            }

            if (null == current || null == bindable)
            {
                return;
            }

            if (null != current.AttachedObject)
            {
                throw new InvalidOperationException("Cannot Host BehaviorCollection Multiple Times");
            }

            /*var element = bindable as FrameworkElement;

            if (null == element)
            {
                throw new InvalidOperationException("Can only host BehaviorCollection on FrameworkElement");
            }*/

            current.Attach(bindable);
        }
    }
}
