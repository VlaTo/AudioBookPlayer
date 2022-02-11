using Android.App;
using Android.Content;
using AndroidX.AppCompat.App;
using System;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace AudioBookPlayer.App.Views.Activities
{
    [Activity(Theme = "@style/CustomSplashScreen", MainLauncher = true, NoHistory = true)]
    public class SplashActivity : AppCompatActivity
    {
        private Task startupTask;

        protected override void OnResume()
        {
            base.OnResume();

            startupTask = Task.Run(DoWork);
        }

        private async Task DoWork()
        {
            try
            {
                await AppActions.SetAsync(
                    new AppAction("app_info", "App Info")
                );
            }
            catch (FeatureNotSupportedException exception)
            {
                Console.WriteLine(exception);
                throw;
            }

            StartMainActivity();
        }

        private void StartMainActivity()
        {
            var intent = new Intent(Application.Context, typeof(MainActivity));
            StartActivity(intent);
        }
    }
}