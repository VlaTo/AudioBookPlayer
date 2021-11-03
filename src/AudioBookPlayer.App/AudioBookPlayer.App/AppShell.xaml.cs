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
        public AppShell()
        {
            InitializeComponent();
            
            Routing.RegisterRoute(Routes.PlayerPageRoute, typeof(PlayerControlPage));
        }

        private void OnMenuItemClicked(object sender, EventArgs e)
        {
            //await Current.GoToAsync("//LoginPage");
        }

        private void OnMultiTapToExitBehaviorOnShowHintMessage(object sender, EventArgs e)
        {
            var toastService = AudioBookPlayerApplication.Instance.DependencyContainer.GetInstance<IPlatformToastService>();
            toastService.ShowShortMessage(AppResources.ExitToastMessage);
        }
    }
}
