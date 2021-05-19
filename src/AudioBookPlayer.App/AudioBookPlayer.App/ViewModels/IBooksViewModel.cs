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

        Command<AudioBookViewModel> PlayBook
        {
            get;
        }

        InteractionRequest<PlayBookInteractionRequestContext> PlayBookRequest
        {
            get;
        }
    }
}