using System;
using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.Content;
using AudioBookPlayer.App.Core;
using AudioBookPlayer.App.Droid.Services;
using AudioBookPlayer.App.Services;
using LibraProgramming.Xamarin.Dependency.Container;
using LibraProgramming.Xamarin.Popups.Platforms.Android;
using Xamarin.Forms;
using Environment = Android.OS.Environment;

namespace AudioBookPlayer.App.Droid
{
    [Activity(Theme = "@style/MainTheme", ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize )]
    public class MainActivity : Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
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

            var ismounded = String.Equals(Environment.ExternalStorageState, Environment.MediaMounted);
            if (ismounded)
            {
                var dirs = ContextCompat.GetExternalFilesDirs(ApplicationContext, null);
                
                if (dirs != null && dirs.Length > 0)
                {
                    var primaryExternalStorage = dirs[0];

                }
            }

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
                container.Register<IStorageSourceService, StorageSourceService>(InstanceLifetime.Singleton);
            }
        }
    }
}