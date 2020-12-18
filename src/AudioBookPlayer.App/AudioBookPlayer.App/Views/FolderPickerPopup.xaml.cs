using AudioBookPlayer.App.Core.Attributes;
using AudioBookPlayer.App.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AudioBookPlayer.App.Views
{
    [ViewModel(typeof(FolderPickerViewModel))]
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class FolderPickerPopup : ContentPage
    {
        public FolderPickerPopup()
        {
            InitializeComponent();
        }

        private async void OnApplyRequest(object sender, CloseInteractionRequestContext context)
        {
            await Shell.Current.Navigation.PopModalAsync();
        }
    }
}