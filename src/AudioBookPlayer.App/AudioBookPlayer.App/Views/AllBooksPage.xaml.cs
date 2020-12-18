using AudioBookPlayer.App.Core.Attributes;
using AudioBookPlayer.App.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AudioBookPlayer.App.Views
{
    [ViewModel(typeof(AllBooksViewModel))]
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AllBooksPage : ContentPage
    {
        public AllBooksPage()
        {
            InitializeComponent();
        }
    }
}