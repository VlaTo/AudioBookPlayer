using System;
using AudioBookPlayer.App.Core.Attributes;
using AudioBookPlayer.App.ViewModels;
using Xamarin.Forms;
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

        private async void OnPickChapterRequest(object _, PickChapterRequestContext context, Action callback)
        {
            var popup = new ChapterPickerPopup(context, callback);

            await Shell.Current.Navigation.PushModalAsync(popup);
        }
    }
}