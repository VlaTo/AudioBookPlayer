using System.Collections.ObjectModel;
using LibraProgramming.Xamarin.Interaction;
using Xamarin.Forms;

namespace AudioBookPlayer.App.ViewModels
{
    public interface IBooksViewModel
    {
        ObservableCollection<BookPreviewViewModel> Books
        {
            get;
        }

        Command<BookPreviewViewModel> StartPlay
        {
            get;
        }

        InteractionRequest<StartPlayInteractionRequestContext> StartPlayRequest
        {
            get;
        }
    }
}