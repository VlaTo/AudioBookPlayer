using AudioBookPlayer.App.Core;
using AudioBookPlayer.App.Views;
using System;
using AudioBookPlayer.App.Resources;
using AudioBookPlayer.App.Services;
using Xamarin.Forms;

namespace AudioBookPlayer.App
{
    public partial class AppShell
    {
        private readonly IPlatformToastService toastService;

        public AppShell()
        {
            InitializeComponent();
            
            Routing.RegisterRoute(Routes.PlayerPageRoute, typeof(PlayerControlPage));

            toastService = AudioBookPlayerApp.Current.DependencyContainer.GetInstance<IPlatformToastService>();
        }

        private void OnMenuItemClicked(object sender, EventArgs e)
        {
            //await Current.GoToAsync("//LoginPage");
        }

        private void OnMultiTapToExitBehaviorOnShowHintMessage(object sender, EventArgs e)
        {
            toastService.ShowShortMessage(AppResources.ExitToastMessage);
        }
    }
}
