using System;
using AudioBookPlayer.App.Core;
using AudioBookPlayer.App.Data;
using AudioBookPlayer.App.Services;
using LibraProgramming.Xamarin.Dependency.Container;
using LibraProgramming.Xamarin.Popups.Services;
using Microsoft.Data.Sqlite;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]

[assembly: ExportFont("font-awesome-regular.otf", Alias = "FontAwesomeRegular")]
[assembly: ExportFont("font-awesome-solid.otf", Alias = "FontAwesomeSolid")]

namespace AudioBookPlayer.App
{
    public partial class AudioBookPlayerApp
    {
        internal new static AudioBookPlayerApp Current => (AudioBookPlayerApp) Application.Current;

        public AudioBookPlayerApp(IPlatformInitializer platformInitializer)
            : base(platformInitializer)
        {
            InitializeComponent();

            MainPage = new AppShell();
        }

        protected override void Initialize()
        {
            base.Initialize();
            InitializeDatabase();
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
            container.Register<ApplicationSettings>(InstanceLifetime.Singleton);
            container.Register<IAudioBookFactoryProvider, AudioBookFactoryProvider>(InstanceLifetime.Singleton);
            container.Register<IBookShelfDataContext, SqLiteBookShelfDataContext>(InstanceLifetime.Singleton, createimmediate: true);
            container.Register<IBookShelfProvider, BookShelfProvider>(InstanceLifetime.Singleton);
            container.Register<IPopupService, PopupService>(InstanceLifetime.Singleton);
        }

        private static void InitializeDatabase()
        {
            var db = Current.DependencyContainer.GetInstance<IBookShelfDataContext>();

            try
            {
                db.Initialize();
            }
            catch (SqliteException exception)
            {
                Console.WriteLine(exception);
                throw;
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                throw;
            }
        }
    }
}
