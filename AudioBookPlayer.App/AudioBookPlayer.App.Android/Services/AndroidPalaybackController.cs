using Android.App;
using Android.Content;
using AudioBookPlayer.App.Core.Services;

namespace AudioBookPlayer.App.Droid.Services
{
    internal sealed class AndroidPalaybackController : IPlaybackController
    {
        public AndroidPalaybackController()
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