using AudioBookPlayer.App.ViewModels;
using AudioBookPlayer.App.Core.Attributes;
using Xamarin.Forms.Xaml;

namespace AudioBookPlayer.App.Views
{
    [ViewModel(typeof(PlayerControlViewModel))]
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PlayerControlPage
    {
        public PlayerControlPage()
        {
            InitializeComponent();
        }
    }
}