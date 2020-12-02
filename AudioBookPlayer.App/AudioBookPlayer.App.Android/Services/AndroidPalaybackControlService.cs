using Android.App;
using Android.Content;
using Android.Net;
using AndroidX.Core.App;
using AudioBookPlayer.App.Core.Services;

namespace AudioBookPlayer.App.Droid.Services
{
    internal sealed class AndroidPalaybackControlService : IPlaybackControlService
    {
        public AndroidPalaybackControlService()
        {
        }

        public void StartPlay(string filename)
        {
            var intent = new Intent(AndroidPlaybackService.ActionPlay);
            
            intent.SetPackage(Application.Context.PackageName);
            intent.PutExtra("Filename", filename);
            
            Application.Context.StartService(intent);
        }

        public void ShowNotification()
        {
        }
    }
}