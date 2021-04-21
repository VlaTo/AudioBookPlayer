using AudioBookPlayer.App.Views;
using System;
using Xamarin.Forms;

namespace AudioBookPlayer.App
{
    public partial class AppShell
    {
        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute(nameof(ItemDetailPage), typeof(ItemDetailPage));
            Routing.RegisterRoute(nameof(NewItemPage), typeof(NewItemPage));
        }

        private async void OnMenuItemClicked(object sender, EventArgs e)
        {
            await Current.GoToAsync("//LoginPage");
        }
    }
}
