using System;
using System.Reactive.Linq;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;

namespace AudioBookPlayer.App.Views.Activities
{
    [Activity(Label = "ShareActivity")]
    public class ShareActivity : Activity
    {
        private IDisposable backButtonClickSubscription;
        private Button backButton;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            if (null != Intent)
            {
                var test = Intent.GetIntExtra("Test", -1);

                if (-1 < test)
                {
                    ;
                }
            }

            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.activity_share);

            backButton = FindViewById<Button>(Resource.Id.back_button);

            if (null != backButton)
            {
                backButton.Text = "Finish";
                backButtonClickSubscription = Observable
                    .FromEventPattern(
                        handler => backButton.Click += handler,
                        handler => backButton.Click -= handler
                    )
                    .Subscribe(
                        pattern => SubmitAndFinish()
                    );
            }
        }

        protected override void OnDestroy()
        {
            backButtonClickSubscription?.Dispose();

            base.OnDestroy();
        }

        private void SubmitAndFinish()
        {
            var intent = new Intent(Application.Context, typeof(MainActivity));
            
            intent.PutExtra("Response", 100);

            SetResult(Result.Ok, intent);
            Finish();
        }
    }
}