using LibraProgramming.Xamarin.Interaction.Contracts;
using System;

namespace LibraProgramming.Xamarin.Interaction
{
    public class InteractionRequest : IInteractionRequest
    {
        public event EventHandler<InteractionRequestedEventArgs> Raised;

        public InteractionRequest()
        {
        }

        protected void DoRaiseEvent(InteractionRequestedEventArgs e)
        {
            var handler = Raised;

            if (null != handler)
            {
                handler.Invoke(this, e);
            }
        }
    }

    public class InteractionRequest<TContext> : InteractionRequest
        where TContext : InteractionRequestContext
    {
        public InteractionRequest()
        {
        }

        public void Raise(TContext context, Action callback = null)
        {
            DoRaiseEvent(new InteractionRequestedEventArgs(context, callback ?? Callback.Empty));
        }
    }
}
