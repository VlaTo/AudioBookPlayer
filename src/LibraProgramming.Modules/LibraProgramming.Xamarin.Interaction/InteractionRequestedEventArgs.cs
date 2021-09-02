using System;

namespace LibraProgramming.Xamarin.Interaction
{
    public sealed class InteractionRequestedEventArgs : EventArgs
    {
        public InteractionRequestContext Context
        {
            get;
        }

        public Action Callback
        {
            get;
        }

        public InteractionRequestedEventArgs(InteractionRequestContext context, Action callback = null)
        {
            Context = context;
            Callback = callback;
        }
    }
}
