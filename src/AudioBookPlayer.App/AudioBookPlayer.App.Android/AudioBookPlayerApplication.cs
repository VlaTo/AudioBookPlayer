using Android.App;
using Android.Runtime;
using System;

namespace AudioBookPlayer.App.Droid
{
    [Application(Theme = "@style/MainTheme")]
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