using AudioBookPlayer.App.Core;
using AudioBookPlayer.App.Services;
using LibraProgramming.Xamarin.Dependency.Container;
using Xamarin.Forms;

[assembly: ExportFont("fontello.ttf", Alias = "Icons")]

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
        }
    }
}
