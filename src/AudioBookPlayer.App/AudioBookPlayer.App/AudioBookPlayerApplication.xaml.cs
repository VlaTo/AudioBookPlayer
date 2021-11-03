using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using AudioBookPlayer.App.Core;
using AudioBookPlayer.App.Domain.Services;
using AudioBookPlayer.App.Persistence.LiteDb;
using AudioBookPlayer.App.Services;
using LibraProgramming.Xamarin.Dependency.Container;
using LibraProgramming.Xamarin.Popups.Services;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace AudioBookPlayer.App
{
    public partial class AudioBookPlayerApplication
    {
        public static AudioBookPlayerApplication Instance => (AudioBookPlayerApplication)Current;

        private readonly TaskExecutionMonitor executionMonitor;

        public AudioBookPlayerApplication(IPlatformInitializer initializer)
            : base(initializer)
        {
            InitializeComponent();

            executionMonitor = new TaskExecutionMonitor(RegisterExtraActionsAsync);

            DependencyContainer.Register<ApplicationSettings>(InstanceLifetime.Singleton);
            DependencyContainer.Register<IMediaInfoProviderFactory, MediaInfoProviderFactory>(InstanceLifetime.Singleton);
            DependencyContainer.Register<IActivityTrackerService, ActivityTrackerService>(InstanceLifetime.Singleton);
            DependencyContainer.Register<LiteDbContext>(InstanceLifetime.Singleton);
            //DependencyContainer.Register<AudioBooksLibrary>(InstanceLifetime.CreateNew);
            //DependencyContainer.Register<IBooksService, BooksService>(InstanceLifetime.Singleton);
            DependencyContainer.Register<IPopupService, PopupService>(InstanceLifetime.Singleton);

            // var audioBooksHub = new AudioBooksHub();

            //DependencyContainer.Register<IAudioBooksPublisher>(() => audioBooksHub, InstanceLifetime.Singleton);
            //DependencyContainer.Register<IAudioBooksConsumer>(() => audioBooksHub, InstanceLifetime.Singleton);
            //DependencyContainer.Register(() => DependencyService.Get<IMediaBrowserServiceConnector>(), InstanceLifetime.CreateNew);

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
        
        /*private void InitializeDatabase()
        {
            var db = DependencyContainer.GetInstance<LiteDbContext>();

            try
            {
                db.Initialize();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                throw;
            }
        }*/
    }
}
