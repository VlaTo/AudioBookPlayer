using Android.App;
using Android.Content;
using AndroidX.AppCompat.App;

namespace AudioBookPlayer.App.Android
{
    [Activity(Label = "AudioBookPlayer", Icon = "@mipmap/icon", Theme ="@style/MainSplash", MainLauncher = true, NoHistory = true)]
    public class SplashActivity : AppCompatActivity
    {
        protected override void OnStart()
        {
            base.OnStart();

        }

        // Launches the startup task
        protected override void OnResume()
        {
            base.OnResume();

            StartActivity(new Intent(Application.Context, typeof(MainActivity)));
        }
    }
}