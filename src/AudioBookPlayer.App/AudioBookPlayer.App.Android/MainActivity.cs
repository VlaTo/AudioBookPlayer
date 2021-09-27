using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using AudioBookPlayer.App.Android.Services;
using AudioBookPlayer.App.Core;
using AudioBookPlayer.App.Domain.Providers;
using AudioBookPlayer.App.Domain.Services;
using AudioBookPlayer.App.Services;
using LibraProgramming.Xamarin.Core;
using LibraProgramming.Xamarin.Dependency.Container;
using Xamarin.Forms;
using Forms = Xamarin.Forms.Forms;
using LibraControls = LibraProgramming.Xamarin.Controls.Controls;
using LibraPopups = LibraProgramming.Xamarin.Popups.Popup;

namespace AudioBookPlayer.App.Android
{
    [Activity(Label = "@string/app_name", Icon = "@mipmap/icon", Theme = "@style/MainTheme", ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize )]
    public class MainActivity : Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        private readonly TaskExecutionMonitor serviceConnect;

        public MainActivity()
        {
            serviceConnect = new TaskExecutionMonitor(DoServiceConnectAsync);
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);

            Forms.Init(this, savedInstanceState);
            LibraControls.Init(this, savedInstanceState);
            LibraPopups.Init(this, savedInstanceState);
            
            LoadApplication(new AudioBookPlayerApplication(new AndroidInitializer()));
        }

        protected override void OnStart()
        {
            base.OnStart();
            serviceConnect.Start();
        }

        protected override void OnResume()
        {
            base.OnResume();
            Xamarin.Essentials.Platform.OnResume(this);
        }

        protected override void OnNewIntent(Intent intent)
        {
            base.OnNewIntent(intent);
            Xamarin.Essentials.Platform.OnNewIntent(intent);
        }

        private static Task DoServiceConnectAsync()
        {
            var dependencyContainer = AudioBookPlayerApplication.Instance.DependencyContainer;
            var connector = dependencyContainer.GetInstance<IMediaBrowserServiceConnector>();
            return connector.ConnectAsync();
        }

        /// <summary>
        /// 
        /// </summary>
        private sealed class AndroidInitializer : IPlatformInitializer
        {
            public void RegisterTypes(DependencyContainer container)
            {
                // Xamarin.Forms.Internals.Registrar.Registered.Register();

                container.Register<IMediaBrowserServiceConnector, MediaBrowserServiceConnector>(InstanceLifetime.Singleton);
                container.Register<IPermissionRequestor, PermissionRequestor>(InstanceLifetime.Singleton);
                container.Register<IBooksProvider, BooksProvider>(InstanceLifetime.Singleton);
                container.Register<ICoverService, CoverService>(InstanceLifetime.Singleton);
                container.Register<IPlatformToastService, ToastService>(InstanceLifetime.Singleton);
                container.Register<IDatabasePathProvider, DatabasePathProvider>();
                container.Register<IBookItemsCache, InMemoryBookItemsCache>(InstanceLifetime.Singleton);
            }
        }
    }
}