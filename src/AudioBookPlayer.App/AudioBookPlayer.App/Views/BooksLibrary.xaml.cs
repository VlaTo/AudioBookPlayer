using AudioBookPlayer.App.Core.Attributes;
using AudioBookPlayer.App.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AudioBookPlayer.App.Views
{
    [ViewModel(typeof(BooksLibraryViewModel))]
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BooksLibrary : ContentPage
    {
        public BooksLibrary()
        {
            InitializeComponent();
        }
    }
}