namespace LibraProgramming.Xamarin.Interaction
{
    public class InteractionRequestContext
    {
        public static readonly InteractionRequestContext Empty;

        static InteractionRequestContext()
        {
            Empty = new EmptyInteractionRequestContext();
        }

        private sealed class EmptyInteractionRequestContext : InteractionRequestContext
        {

        }
    }

    public class InteractionRequestContext<T> : InteractionRequestContext
    {
        public T Argument
        {
            get;
        }

        public InteractionRequestContext(T argument)
        {
            Argument = argument;
        }
    }
}
