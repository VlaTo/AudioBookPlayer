using AudioBookPlayer.App.Core.Attributes;
using AudioBookPlayer.App.ViewModels;
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