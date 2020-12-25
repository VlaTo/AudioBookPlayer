﻿using AudioBookPlayer.App.Core;
using AudioBookPlayer.App.Services;
using LibraProgramming.Xamarin.Dependency.Container;
using LibraProgramming.Xamarin.Popups.Services;
using Xamarin.Forms;

//[assembly: ExportFont("fontello.ttf", Alias = "Icons")]
[assembly: ExportFont("font-awesome-regular.otf", Alias = "FontAwesomeRegular")]
[assembly: ExportFont("font-awesome-solid.otf", Alias = "FontAwesomeSolid")]

namespace AudioBookPlayer.App
{
    public partial class AudioBookPlayerApp : BaseApplication
    {
        public static new AudioBookPlayerApp Current => (AudioBookPlayerApp)Application.Current;

        public AudioBookPlayerApp(IPlatformInitializer platformInitializer)
            : base(platformInitializer)
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

        protected override void RegisterTypesCore(DependencyContainer container)
        {
            //base.RegisterTypesCore(container);

            container.Register<ApplicationSettings>(InstanceLifetime.Singleton);
            container.Register<IBookShelfProvider, SqLiteDatabaseBookShelfProvider>(InstanceLifetime.Singleton);
            //container.Register<IPermissionRequestor, PermissionRequestor>(InstanceLifetime.Singleton);
            container.Register<IPopupService, PopupService>(InstanceLifetime.Singleton);
        }
    }
}
