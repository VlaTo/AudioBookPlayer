using System.Collections.ObjectModel;
using LibraProgramming.Xamarin.Interaction;
using Xamarin.Forms;

namespace AudioBookPlayer.App.ViewModels
{
    public interface IBooksViewModel
    {
        ObservableCollection<AudioBookViewModel> Books
        {
            get;
        }

        Command<AudioBookViewModel> StartPlay
        {
            get;
        }

        InteractionRequest<StartPlayInteractionRequestContext> StartPlayRequest
        {
            get;
        }
    }
}