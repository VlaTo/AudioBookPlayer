using AudioBookPlayer.App.Core;
using AudioBookPlayer.App.Data;
using AudioBookPlayer.App.Services;
using LibraProgramming.Xamarin.Dependency.Container;
using LibraProgramming.Xamarin.Popups.Services;
using Microsoft.Data.Sqlite;
using System;
using Xamarin.Forms;

namespace AudioBookPlayer.App
{
    public partial class AudioBookPlayerApp
    {
        internal new static AudioBookPlayerApp Current => (AudioBookPlayerApp)Application.Current;

        public AudioBookPlayerApp(IPlatformInitializer platformInitializer)
            : base(platformInitializer)
        {
            InitializeComponent();

            MainPage = new AppShell();
        }

        protected override void OnStart()
        {
            try
            {
                var db = DependencyContainer.GetInstance<IBookShelfDataContext>();
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
    }
}
