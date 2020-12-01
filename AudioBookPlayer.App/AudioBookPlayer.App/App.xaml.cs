using AudioBookPlayer.App.ViewModels;
using AudioBookPlayer.App.Views;
using Prism;
using Prism.Ioc;
using Xamarin.Forms;

// https://fontello.com/
[assembly: ExportFont("fontello.ttf", Alias = "IconsFont")]

namespace AudioBookPlayer.App
{
    public partial class App
    {
        public App(IPlatformInitializer initializer)
            : base(initializer)
        {
        }

        protected override async void OnInitialized()
        {
            InitializeComponent();

            await NavigationService.NavigateAsync("NavigationPage/MainPage");
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            //containerRegistry.RegisterSingleton<AppInfo, AppInfoImplementation>();

            containerRegistry.RegisterForNavigation<NavigationPage>();
            containerRegistry.RegisterForNavigation<MainPage, MainPageViewModel>();
        }
    }
}
