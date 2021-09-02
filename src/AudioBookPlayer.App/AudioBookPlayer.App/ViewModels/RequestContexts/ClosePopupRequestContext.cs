using LibraProgramming.Xamarin.Interaction;

namespace AudioBookPlayer.App.ViewModels.RequestContexts
{
    public class ClosePopupRequestContext : InteractionRequestContext
    {
        public bool Animated
        {
            get;
        }

        public ClosePopupRequestContext(bool animated)
        {
            Animated = animated;
        }
    }
}