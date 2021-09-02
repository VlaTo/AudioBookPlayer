using LibraProgramming.Xamarin.Interaction;
using LibraProgramming.Xamarin.Interaction.Contracts;
using System.Collections.Generic;
using Xamarin.Forms;

namespace LibraProgramming.Xamarin.Interactivity
{
    public partial class RequestTrigger : InteractivityBase, IRequestTrigger
    {
        public static readonly BindableProperty RequestProperty;

        private readonly RequestTriggerActionCollection actions;

        public IInteractionRequest Request
        {
            get => (IInteractionRequest)GetValue(RequestProperty);
            set => SetValue(RequestProperty, value);
        }

        public IList<RequestTriggerAction> Actions => actions;

        public RequestTrigger()
        {
            actions = new RequestTriggerActionCollection(this);
        }

        static RequestTrigger()
        {
            RequestProperty = BindableProperty.Create(
                nameof(Request),
                typeof(IInteractionRequest),
                typeof(InteractionRequestTrigger),
                null,
                propertyChanged: OnRequestPropertyChanged
            );
        }

        protected override void OnAttached()
        {
            base.OnAttached();

            foreach(var action in actions)
            {
                action.AttachedObject = AttachedObject;
            }
        }

        protected override void OnDetached()
        {
            base.OnDetached();

            foreach (var action in actions)
            {
                action.AttachedObject = null;
            }
        }

        private void OnRequestChanged(IInteractionRequest oldValue, IInteractionRequest newValue)
        {
            if (null != oldValue)
            {
                oldValue.Raised -= DoRequestRaised;
            }

            if (null != newValue)
            {
                newValue.Raised += DoRequestRaised;
            }
        }

        private void DoRequestRaised(object sender, InteractionRequestedEventArgs e)
        {
            var request = (IInteractionRequest)sender;

            foreach (var action in Actions)
            {
                action.Invoke(request, e.Context, e.Callback);
            }
        }

        private void DoActionInserted(int index, RequestTriggerAction action)
        {
            action.AttachedObject = AttachedObject;
        }

        private void DoActionRemoved(int index, RequestTriggerAction action)
        {
            action.AttachedObject = null;
        }

        private static void OnRequestPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            ((RequestTrigger)bindable).OnRequestChanged((IInteractionRequest)oldValue, (IInteractionRequest)newValue);
        }
    }
}
