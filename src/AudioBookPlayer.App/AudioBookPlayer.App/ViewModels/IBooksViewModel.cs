using System.Collections.ObjectModel;
using LibraProgramming.Xamarin.Interaction;
using Xamarin.Forms;

namespace AudioBookPlayer.App.ViewModels
{
    public interface IBooksViewModel
    {
        ObservableCollection<BookItemViewModel> Books
        {
            get;
        }

        Command<BookItemViewModel> StartPlay
        {
            get;
        }

        InteractionRequest<StartPlayInteractionRequestContext> StartPlayRequest
        {
            get;
        }
    }
}