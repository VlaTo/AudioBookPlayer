using AudioBookPlayer.App.Core.Attributes;
using AudioBookPlayer.App.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AudioBookPlayer.App.Views
{
    [ViewModel(typeof(SettingsViewModel))]
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SettingsPage : ContentPage
    {
        public SettingsPage()
        {
            InitializeComponent();
        }

        private async void OnSelectFolderRequest(object sender, SourceFolderRequestContext context)
        {
            using (var deferral = context.GetDeferral())
            {
                var page = new FolderPickerPopup();

                await Shell.Current.Navigation.PushModalAsync(page);

                deferral.Complete();
            }
        }
    }
}