using AudioBookPlayer.App.Core.Attributes;
using AudioBookPlayer.App.ViewModels;
using AudioBookPlayer.App.ViewModels.RequestContexts;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using AudioBookPlayer.App.Core.Extensions;
using Xamarin.Essentials;
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

        private void OnPickChapterRequest(object _, PickChapterRequestContext context, Action callback)
        {
            OpenDrawer();
        }

        private async void OnBookmarkRequest(object _, BookmarkRequestContext context, Action callback)
        {
            var page = new BookmarksPage(context, callback);
            await Shell.Current.Navigation.PushModalAsync(page);
        }
        
        private void OpenDrawer()
        {
            Backdrop.IsVisible = true;
            BottomDrawer.IsVisible = true;
        }

        private void CloseDrawer()
        {
            Task.WhenAll(
                    BottomDrawer.TranslateTo(0.0d, 340.0d, easing: Easing.SinOut),
                    Backdrop.FadeTo(0.0d, easing: Easing.Linear)
                )
                .ContinueWith(_ =>
                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        BottomDrawer.IsVisible = false;
                        Backdrop.IsVisible = false;
                    })
                )
                .RunAndForget();
        }

        private void OnBoxViewTapGestureRecognizerTapped(object sender, EventArgs e)
        {
            CloseDrawer();
        }

        private void OnSectionsSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CloseDrawer();
        }

        private void OnFramePanGestureRecognizerPanUpdated(object sender, PanUpdatedEventArgs e)
        {
            switch (e.StatusType)
            {
                case GestureStatus.Running:
                {
                    if (0.0d < e.TotalY)
                    {
                        BottomDrawer.TranslationY = BottomDrawer.TranslationY + e.TotalY;
                    }

                    break;
                }

                case GestureStatus.Canceled:
                case GestureStatus.Completed:
                {
                    if (100.0d < (BottomDrawer.TranslationY - 16.0d))
                    {
                        CloseDrawer();
                    }
                    else
                    {
                        BottomDrawer
                            .TranslateTo(0.0d, 16.0d, easing: Easing.SpringIn)
                            .RunAndForget();
                    }

                    break;
                }
            }
        }
    }
}