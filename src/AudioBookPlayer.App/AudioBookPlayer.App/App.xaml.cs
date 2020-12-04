using AudioBookPlayer.App.Services;
using Xamarin.Forms;

[assembly: ExportFont("fontello.ttf", Alias = "Icons")]

namespace AudioBookPlayer.App
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            DependencyService.Register<MockDataStore>();

            MainPage = new AppShell();
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
