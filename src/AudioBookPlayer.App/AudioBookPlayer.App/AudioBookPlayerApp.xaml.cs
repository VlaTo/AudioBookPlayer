using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using AudioBookPlayer.App.Core;
using AudioBookPlayer.App.Data;
using AudioBookPlayer.App.Services;
using LibraProgramming.Xamarin.Dependency.Container;
using LibraProgramming.Xamarin.Popups.Services;
using Microsoft.Data.Sqlite;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]

[assembly: ExportFont("fa-solid-900.ttf", Alias = "FASolid")]
[assembly: ExportFont("fa-regular-400.ttf", Alias = "FARegular")]
[assembly: ExportFont("fa-brands-400.ttf", Alias = "FABrands")]

namespace AudioBookPlayer.App
{
    public partial class AudioBookPlayerApp
    {
        internal new static AudioBookPlayerApp Current => (AudioBookPlayerApp) Application.Current;

        private readonly TaskExecutionMonitor executionMonitor;

        public AudioBookPlayerApp(IPlatformInitializer platformInitializer)
            : base(platformInitializer)
        {
            InitializeComponent();

            executionMonitor = new TaskExecutionMonitor(RegisterExtraActionsAsync);

            MainPage = new AppShell();
        }

        protected override void Initialize()
        {
            base.Initialize();
            InitializeDatabase();
        }

        protected override void OnStart()
        {
            Debug.WriteLine($"[AudioBookPlayerApp] [OnStart] Execute");
            executionMonitor.Start();
        }

        protected override void OnSleep()
        {
            Debug.WriteLine($"[AudioBookPlayerApp] [OnSleep] Execute");
        }

        protected override void OnResume()
        {
            Debug.WriteLine($"[AudioBookPlayerApp] [OnResume] Execute");
        }

        protected override void RegisterTypesCore(DependencyContainer container)
        {
            container.Register<ApplicationSettings>(InstanceLifetime.Singleton);
            container.Register<IAudioBookFactoryProvider, AudioBookFactoryProvider>(InstanceLifetime.Singleton);
            container.Register<IBookShelfDataContext, SqLiteBookShelfDataContext>(InstanceLifetime.Singleton, createimmediate: true);
            container.Register<IBookShelfProvider, BookShelfProvider>(InstanceLifetime.Singleton);
            container.Register<IPopupService, PopupService>(InstanceLifetime.Singleton);
        }

        private Task RegisterExtraActionsAsync()
        {
            var actions = new List<AppAction>();

            if (true)
            {
                actions.Add(new AppAction("continue_play", "Continue Play"));
            }

            return RegisterAppActionsAsync(actions);
        }

        private async Task RegisterAppActionsAsync(ICollection<AppAction> actions)
        {
            if (0 == actions.Count)
            {
                return;
            }

            try
            {
                // register extra app actions
                await AppActions.SetAsync(actions);
                // attach app action handler
                AppActions.OnAppAction += OnExtraAppAction;
            }
            catch (FeatureNotSupportedException)
            {
                Debug.Fail($"Can't register extra AppActions");
            }
        }

        private void OnExtraAppAction(object sender, AppActionEventArgs e)
        {
            if (Application.Current != this && Application.Current is AudioBookPlayerApp app)
            {
                AppActions.OnAppAction -= app.OnExtraAppAction;
                return;
            }

            switch (e.AppAction.Id)
            {
                case "continue_play":
                {
                    
                    Debug.WriteLine($"App Action: \"{e.AppAction.Title}\" received");

                    break;
                }
            }
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
