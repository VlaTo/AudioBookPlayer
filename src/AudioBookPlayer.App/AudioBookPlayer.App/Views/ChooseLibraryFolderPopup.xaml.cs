using AudioBookPlayer.App.Core.Attributes;
using AudioBookPlayer.App.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AudioBookPlayer.App.Views
{
    [ViewModel(typeof(ChooseLibraryFolderViewModel))]
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ChooseLibraryFolderPopup : ContentPage
    {
        public ChooseLibraryFolderPopup()
        {
            InitializeComponent();
        }
    }
}