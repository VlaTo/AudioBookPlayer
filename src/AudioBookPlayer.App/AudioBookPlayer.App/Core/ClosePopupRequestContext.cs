using LibraProgramming.Xamarin.Interaction;

namespace AudioBookPlayer.App.Core
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