using System;
using System.Diagnostics;
using AudioBookPlayer.App.Core.Attributes;
using AudioBookPlayer.App.ViewModels;
using AudioBookPlayer.App.ViewModels.RequestContexts;
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

            callback.Invoke();
        }

        private async void OnBookmarkRequest(object _, BookmarkRequestContext context, Action callback)
        {
            var page = new BookmarksPage(context, callback);
            await Shell.Current.Navigation.PushModalAsync(page);
        }
    }
}