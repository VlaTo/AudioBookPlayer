using AudioBookPlayer.App.Core.Attributes;
using AudioBookPlayer.App.ViewModels;
using System;
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

        private async void OnSelectFolderRequest(object sender, SourceFolderRequestContext context, Action callback)
        {
            var page = new FolderPickerPopup();

            await Shell.Current.Navigation.PushModalAsync(page);

            await page.InitializePathAsync(context.LibraryRootFolder);
        }
    }
}