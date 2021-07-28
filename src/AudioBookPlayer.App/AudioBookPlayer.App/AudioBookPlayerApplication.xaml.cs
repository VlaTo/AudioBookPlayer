using AudioBookPlayer.App.Core;
using AudioBookPlayer.App.Data;
using AudioBookPlayer.App.Services;
using LibraProgramming.Xamarin.Dependency.Container;
using LibraProgramming.Xamarin.Popups.Services;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using AudioBookPlayer.App.Domain.Services;
using AudioBookPlayer.App.Persistence;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace AudioBookPlayer.App
{
    public partial class AudioBookPlayerApplication
    {
        internal new static AudioBookPlayerApplication Current => (AudioBookPlayerApplication) Application.Current;

        private readonly TaskExecutionMonitor executionMonitor;

        public AudioBookPlayerApplication(IPlatformInitializer initializer)
            : base(initializer)
        {
            InitializeComponent();

            executionMonitor = new TaskExecutionMonitor(RegisterExtraActionsAsync);

            DependencyContainer.Register<ApplicationSettings>(InstanceLifetime.Singleton);
            DependencyContainer.Register<IAudioBookFactoryProvider, AudioBookFactoryProvider>(InstanceLifetime.Singleton);
            DependencyContainer.Register<IMediaLibrary, MediaLibrary>(InstanceLifetime.Singleton);
            DependencyContainer.Register<IMediaLibraryDataContext, SqLiteMediaLibraryDataContext>(InstanceLifetime.Singleton);
            DependencyContainer.Register<IPopupService, PopupService>(InstanceLifetime.Singleton);

            var audioBooksHub = new AudioBooksHub();

            DependencyContainer.Register<IAudioBooksPublisher>(() => audioBooksHub, InstanceLifetime.Singleton);
            DependencyContainer.Register<IAudioBooksConsumer>(() => audioBooksHub, InstanceLifetime.Singleton);

            MainPage = new AppShell();
        }

        protected override void OnStart()
        {
            executionMonitor.Start();
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
        
        private Task RegisterExtraActionsAsync()
        {
            var actions = new List<AppAction>();

            if (true)
            {
                actions.Add(new AppAction("continue_play", "Extra action #1"));
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

            await Task.Delay(TimeSpan.FromSeconds(3.0d));

            InitializeDatabase();
        }
        
        private void OnExtraAppAction(object sender, AppActionEventArgs e)
        {
            if (Application.Current != this && Application.Current is AudioBookPlayerApplication app)
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
        
        private void InitializeDatabase()
        {
            var db = DependencyContainer.GetInstance<IMediaLibraryDataContext>();

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
