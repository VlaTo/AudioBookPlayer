using AudioBookPlayer.App.Core.Attributes;
using AudioBookPlayer.App.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AudioBookPlayer.App.Views
{
    [ViewModel(typeof(BooksLibraryViewModel))]
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BooksLibraryPage : ContentPage
    {
        public BooksLibraryPage()
        {
            InitializeComponent();
        }
    }
}