using LibraProgramming.Xamarin.Interaction;
using LibraProgramming.Xamarin.Interaction.Contracts;
using System;
using Xamarin.Forms;

namespace LibraProgramming.Xamarin.Interactivity
{
    public class RequestTriggerAction : BindableObject, IRequestTriggerAction
    {
        protected internal BindableObject AttachedObject
        {
            get;
            internal set;
        }

        protected RequestTriggerAction()
        {
        }

        public virtual void Invoke(IInteractionRequest request, InteractionRequestContext context, Action callback)
        {
        }
    }

    public abstract class RequestTriggerAction<TContext> : RequestTriggerAction
        where TContext : InteractionRequestContext
    {
        public override void Invoke(IInteractionRequest request, InteractionRequestContext context, Action callback)
        {
            Invoke(request, (TContext)context, callback);
        }

        protected abstract void Invoke(IInteractionRequest request, TContext context, Action callback);
    }
}
