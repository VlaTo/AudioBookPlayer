using Android.App;
using Android.Content;
using Android.OS;
using AndroidX.AppCompat.App;

namespace AudioBookPlayer.App.Android
{
    [Activity(Label = "@string/app_name", Icon = "@mipmap/icon", Theme ="@style/MainSplash", MainLauncher = true, NoHistory = true)]
    public class SplashActivity : AppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            
            StartActivity(new Intent(Application.Context, typeof(MainActivity)));

            Finish();
        }
    }
}