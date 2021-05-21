using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using AudioBookPlayer.App.Core;
using AudioBookPlayer.App.Droid.Services;
using AudioBookPlayer.App.Services;
using LibraProgramming.Xamarin.Dependency.Container;
using LibraProgramming.Xamarin.Popups.Platforms.Android;
using TheControls = LibraProgramming.Xamarin.Popups.Platforms.Android.Controls;
using Xamarin.Forms;
using IPlaybackService = AudioBookPlayer.App.Droid.Services.IPlaybackService;

namespace AudioBookPlayer.App.Droid
{
    [Activity(Theme = "@style/MainTheme", ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize )]
    public class MainActivity : Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        internal PlaybackServiceConnection PlaybackServiceConnection
        {
            get;
            private set;
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        internal void OnPlaybackServiceConnected()
        {
            System.Diagnostics.Debug.WriteLine("[MainActivity] [OnPlaybackServiceConnected] Executed");
        }

        internal void OnPlaybackServiceDisconnected()
        {
            System.Diagnostics.Debug.WriteLine("[MainActivity] [OnPlaybackServiceDisconnected] Executed");
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);

            //Xamarin.Forms.Forms.SetFlags("Expander_Experimental");
            
            Forms.Init(this, savedInstanceState);
            TheControls.Init(this, savedInstanceState);
            Popup.Init(this, savedInstanceState);

            LoadApplication(new AudioBookPlayerApp(new AndroidInitializer()));
        }

        protected override void OnStart()
        {
            base.OnStart();

            if (null == PlaybackServiceConnection)
            {
                PlaybackServiceConnection = new PlaybackServiceConnection(this);
            }

            var intent = new Intent(this, typeof(PlaybackService));

            BindService(intent, PlaybackServiceConnection, Bind.AutoCreate);
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

        /// <summary>
        /// 
        /// </summary>
        private sealed class AndroidInitializer : IPlatformInitializer
        {
            public void RegisterTypes(DependencyContainer container)
            {
                container.Register<IPermissionRequestor, PermissionRequestor>(InstanceLifetime.Singleton);
                container.Register<IMediaService, MediaService>(InstanceLifetime.Singleton);
                container.Register<IStorageSourceService, StorageSourceService>(InstanceLifetime.Singleton);
                container.Register<IPlaybackController>(GetPlaybackController, InstanceLifetime.Singleton);
                container.Register<IToastService, ToastService>(InstanceLifetime.Singleton);
            }

            private static PlaybackController GetPlaybackController()
            {
                var mainActivity = (MainActivity) Xamarin.Essentials.Platform.CurrentActivity;
                var connection = mainActivity.PlaybackServiceConnection;

                return new PlaybackController(connection.Binder.Service);
            }
        }
    }
}