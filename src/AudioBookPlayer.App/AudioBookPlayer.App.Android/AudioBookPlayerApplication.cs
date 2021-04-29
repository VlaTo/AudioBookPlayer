using Android.App;
using Android.Runtime;
using System;

/*
[assembly: Preserve(typeof(System.Linq.Queryable), AllMembers = true)]
[assembly: Preserve(typeof(System.DateTime), AllMembers = true)]
[assembly: Preserve(typeof(System.Linq.Enumerable), AllMembers = true)]
[assembly: Preserve(typeof(System.Linq.IQueryable), AllMembers = true)]

[assembly: UsesPermission(Android.Manifest.Permission.AccessMediaLocation)]
[assembly: UsesPermission(Android.Manifest.Permission.MediaContentControl)]
[assembly: UsesPermission(Android.Manifest.Permission.ReadExternalStorage)]
[assembly: UsesPermission(Android.Manifest.Permission.ManageDocuments)]
[assembly: UsesPermission(Android.Manifest.Permission.WakeLock)]
[assembly: UsesPermission(Android.Manifest.Permission.ForegroundService)]
*/

namespace AudioBookPlayer.App.Droid
{
    [Application(Theme = "@style/MainTheme", Label = "@string/app_name")]
    public sealed class AudioBookPlayerApplication : Application
    {
        public AudioBookPlayerApplication(IntPtr javaReference, JniHandleOwnership transfer)
            : base(javaReference, transfer)
        {
        }

        public override void OnCreate()
        {
            base.OnCreate();

            Xamarin.Essentials.Platform.Init(this);
        }
    }
}