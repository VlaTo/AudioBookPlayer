using AudioBookPlayer.App.Views;
using System;
using AudioBookPlayer.App.Core;
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
    }
}
