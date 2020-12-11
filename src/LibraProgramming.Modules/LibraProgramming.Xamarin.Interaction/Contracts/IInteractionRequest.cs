using System;

namespace LibraProgramming.Xamarin.Interaction.Contracts
{
    public interface IInteractionRequest
    {
        event EventHandler<InteractionRequestedEventArgs> Raised;
    }
}
