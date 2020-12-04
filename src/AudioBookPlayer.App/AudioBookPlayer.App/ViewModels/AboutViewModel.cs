using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace AudioBookPlayer.App.ViewModels
{
    public class AboutViewModel : ViewModelBase
    {
        public ICommand RefreshLibrary
        {
            get;
        }

        public AboutViewModel()
        {
            //RefreshLibrary = new Command(async () => await Browser.OpenAsync("https://aka.ms/xamarin-quickstart"));
            RefreshLibrary = new Command(DoRefreshLibrary);
        }

        private async void DoRefreshLibrary()
        {
            await Task.CompletedTask;
        }
    }
}