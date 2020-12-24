using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Widget;
using AudioBookPlayer.App.Core;
using AudioBookPlayer.App.Droid.Services;
using AudioBookPlayer.App.Services;
using LibraProgramming.Xamarin.Dependency.Container;
using LibraProgramming.Xamarin.Popups.Platforms.Android;
using Xamarin.Forms;

//[assembly: UsesPermission(Android.Manifest.Permission.AccessMediaLocation)]
//[assembly: UsesPermission(Android.Manifest.Permission.MediaContentControl)]
[assembly: UsesPermission(Android.Manifest.Permission.ReadExternalStorage)]
//[assembly: UsesPermission(Android.Manifest.Permission.ManageDocuments)]
[assembly: UsesPermission(Android.Manifest.Permission.WakeLock)]
[assembly: UsesPermission(Android.Manifest.Permission.ForegroundService)]

namespace AudioBookPlayer.App.Droid
{
    [Activity(Label = "AudioBookPlayer.App", Icon = "@mipmap/icon", Theme = "@style/MainTheme", ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize )]
    public class MainActivity : Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        private bool backPressedOnce;

        public MainActivity()
        {
            backPressedOnce = false;
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);

            //Xamarin.Forms.Forms.SetFlags("Expander_Experimental");
            //Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            
            Forms.Init(this, savedInstanceState);
            Controls.Init(this, savedInstanceState);
            Popup.Init(this, savedInstanceState);

            LoadApplication(new AudioBookPlayerApp(new AndroidInitializer()));
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        /*public override void OnBackPressed()
        {
            if (backPressedOnce)
            {
                base.OnBackPressed();

                Java.Lang.JavaSystem.Exit(0);

                return;
            }

            backPressedOnce = true;

            var toast = Toast.MakeText(this, "Press twice to exit", ToastLength.Short);

            toast.Show();

            new Handler().PostDelayed(() => backPressedOnce = false, 2000);
        }*/

        /// <summary>
        /// 
        /// </summary>
        private sealed class AndroidInitializer : IPlatformInitializer
        {
            public void RegisterTypes(DependencyContainer container)
            {
                container.Register<IPermissionRequestor, PermissionRequestor>(InstanceLifetime.Singleton);
                container.Register<IMediaService, MediaService>(InstanceLifetime.Singleton);
            }
        }
    }
}