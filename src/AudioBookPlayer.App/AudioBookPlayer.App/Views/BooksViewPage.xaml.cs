using AudioBookPlayer.App.Core;
using AudioBookPlayer.App.ViewModels;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AudioBookPlayer.App.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BooksViewPage
    {
        public BooksViewPage()
        {
            InitializeComponent();
        }

        protected virtual async void OnPlayBookRequest(object sender, StartPlayInteractionRequestContext context, Action callback)
        {
            var uri = $"{Routes.PlayerPageRoute}?{nameof(PlayerControlViewModel.BookId)}={context.EntityId}";
            
            await Shell.Current.GoToAsync(new ShellNavigationState(uri), true);
            
            callback.Invoke();
        }
    }
}