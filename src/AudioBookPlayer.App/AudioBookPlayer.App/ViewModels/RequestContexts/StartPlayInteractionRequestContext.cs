using AudioBookPlayer.App.Domain.Models;
using LibraProgramming.Xamarin.Interaction;

namespace AudioBookPlayer.App.ViewModels.RequestContexts
{
    public class StartPlayInteractionRequestContext : InteractionRequestContext
    {
        public MediaId MediaId
        {
            get;
        }

        public StartPlayInteractionRequestContext(MediaId mediaId)
        {
            MediaId = mediaId;
        }
    }
}