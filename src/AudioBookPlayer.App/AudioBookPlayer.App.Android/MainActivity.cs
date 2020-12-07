﻿
using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using AudioBookPlayer.App.Core;
using LibraProgramming.Xamarin.Dependency.Container;

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
        protected override void OnCreate(Bundle savedInstanceState)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);

            //Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            Xamarin.Forms.Forms.Init(this, savedInstanceState);

            LoadApplication(new AudioBookPlayerApp(new AndroidInitializer()));
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        /// <summary>
        /// 
        /// </summary>
        private sealed class AndroidInitializer : IPlatformInitializer
        {
            public void RegisterTypes(DependencyContainer container)
            {
                ;
            }
        }
    }
}