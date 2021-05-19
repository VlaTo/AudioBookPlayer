using AudioBookPlayer.App.Core.Attributes;
using AudioBookPlayer.App.ViewModels;
using Xamarin.Forms.Xaml;

namespace AudioBookPlayer.App.Views
{
    [ViewModel(typeof(RecentBooksViewModel))]
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RecentBooksPage
    {
        public RecentBooksPage()
        {
            InitializeComponent();
        }
    }
}