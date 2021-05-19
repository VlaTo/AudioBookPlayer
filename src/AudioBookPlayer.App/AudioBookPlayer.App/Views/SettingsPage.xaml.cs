using AudioBookPlayer.App.Core.Attributes;
using AudioBookPlayer.App.ViewModels;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AudioBookPlayer.App.Views
{
    [ViewModel(typeof(SettingsViewModel))]
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SettingsPage
    {
        public SettingsPage()
        {
            InitializeComponent();
        }

        private async void OnSelectFolderRequest(object sender, SourceFolderRequestContext context, Action _)
        {
            var popup = new FolderPickerPopup(context);
            await Shell.Current.Navigation.PushModalAsync(popup);
        }
    }
}