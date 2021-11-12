using AudioBookPlayer.App.Core;
using System;
using AudioBookPlayer.App.ViewModels.RequestContexts;
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
            var uri = $"{Routes.PlayerPageRoute}?mid={context.MediaId}";
            
            await Shell.Current.GoToAsync(new ShellNavigationState(uri), true);
            
            callback.Invoke();
        }
    }
}