using System;
using Android.App;
using Android.Runtime;

namespace AudioBookPlayer.App.Android
{
    /// <summary>
    /// 
    /// </summary>
    [Application(Theme = "@style/MainTheme", Label = "@string/app_name")]
    public sealed class AndroidAudioBookPlayerApplication : Application
    {
        public AndroidAudioBookPlayerApplication(IntPtr javaReference, JniHandleOwnership transfer)
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