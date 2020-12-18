using AudioBookPlayer.App.Core.Attributes;
using AudioBookPlayer.App.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AudioBookPlayer.App.Views
{
    [ViewModel(typeof(RecentBooksViewModel))]
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RecentBooksPage : ContentPage
    {
        public RecentBooksPage()
        {
            InitializeComponent();
        }
    }
}