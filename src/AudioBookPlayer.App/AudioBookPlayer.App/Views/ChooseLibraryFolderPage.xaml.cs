using LibraProgramming.Xamarin.Popups.Platforms.Xamarin;
using Xamarin.Forms.Xaml;

namespace AudioBookPlayer.App.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ChooseLibraryFolderPopup : PopupContentPage
    {
        public ChooseLibraryFolderPopup()
        {
            InitializeComponent();
        }
    }
}