using System;
using AudioBookPlayer.App.Core;
using AudioBookPlayer.App.Data;
using AudioBookPlayer.App.Services;
using LibraProgramming.Xamarin.Dependency.Container;
using LibraProgramming.Xamarin.Popups.Services;
using Microsoft.Data.Sqlite;
using Xamarin.Forms;

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
            container.Register<IBookShelfDataContext, SqLiteBookShelfDataContext>(InstanceLifetime.Singleton);
            container.Register<ApplicationSettings>(InstanceLifetime.Singleton);
            container.Register<IBookShelfProvider, SqLiteDatabaseBookShelfProvider>(InstanceLifetime.Singleton);
            container.Register<IPopupService, PopupService>(InstanceLifetime.Singleton);
        }

        private static void InitializeDatabase()
        {
            var db = Current.DependencyContainer.GetInstance<IBookShelfDataContext>();

            try
            {
                db.EnsureCreated();
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
