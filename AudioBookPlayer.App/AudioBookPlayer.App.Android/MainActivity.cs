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

namespace AudioBookPlayer.App.Droid
{
    [Activity(Theme = "@style/MainTheme", ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);

            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            LoadApplication(new App(new AndroidInitializer()));

            /*var intent = PendingIntent.GetActivity(this, 0, Intent, PendingIntentFlags.UpdateCurrent);

            var notification = new NotificationCompat.Builder(this)
                .SetStyle(new NotificationCompat.BigTextStyle())
                .SetContentTitle("Sample Title")
                .SetContentText("Sample content text")
                .SetSmallIcon(Resource.Drawable.icon)
                .SetContentIntent(intent)
                .Build();

            var manager = NotificationManagerCompat.From(this);

            manager.Notify(0, notification);*/
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
        }
    }
}

