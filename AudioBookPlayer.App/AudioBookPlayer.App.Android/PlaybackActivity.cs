using Android.App;
using AndroidX.AppCompat.App;

namespace AudioBookPlayer.App.Droid
{
    [Activity(MainLauncher = false, NoHistory = true)]
    public class PlaybackActivity : AppCompatActivity
    {
        protected override void OnStart()
        {
            System.Diagnostics.Debug.WriteLine($"[PlaybackActivity] [OnStart]");

            base.OnStart();
        }
    }
}