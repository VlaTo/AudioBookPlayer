using Android.App;
using Android.Content.PM;
using Android.OS;
using AudioBookPlayer.App.Core.Services;
using AudioBookPlayer.App.Droid.Services;
using Prism;
using Prism.Ioc;

[assembly: UsesPermission(Android.Manifest.Permission.AccessMediaLocation)]
[assembly: UsesPermission(Android.Manifest.Permission.MediaContentControl)]
[assembly: UsesPermission(Android.Manifest.Permission.ReadExternalStorage)]
[assembly: UsesPermission(Android.Manifest.Permission.ManageDocuments)]
[assembly: UsesPermission(Android.Manifest.Permission.WakeLock)]

namespace AudioBookPlayer.App.Droid
{
    [Activity(Theme = "@style/MainTheme", ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize)]
    public class MainActivity : Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);

            Xamarin.Forms.Forms.Init(this, savedInstanceState);

            LoadApplication(new App(new AndroidInitializer()));
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }

    public class AndroidInitializer : IPlatformInitializer
    {
        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            // Register any platform specific implementations
            containerRegistry.Register<ISourceStreamProvider, AssetStreamProvider>();
            containerRegistry.RegisterSingleton<IPlaybackControlService, AndroidPalaybackControlService>();
            //containerRegistry.RegisterSingleton<IPlaybackService, AndroidPlaybackService>();
        }
    }
}

