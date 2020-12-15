using LibraProgramming.Xamarin.Interaction;
using LibraProgramming.Xamarin.Interaction.Contracts;
using System;

namespace LibraProgramming.Xamarin.Interactivity
{
    public interface IRequestTriggerAction
    {
        void Invoke(IInteractionRequest request, InteractionRequestContext context, Action callback);
    }
}
