using AudioBookPlayer.App.Core.Attributes;
using AudioBookPlayer.App.ViewModels;
using Xamarin.Forms.Xaml;

namespace AudioBookPlayer.App.Views
{
    [ViewModel(typeof(LibraryViewModel))]
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LibraryPage
    {
        public LibraryPage()
        {
            InitializeComponent();
        }
    }
}