﻿#nullable enable

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using AndroidX.AppCompat.App;
using AudioBookPlayer.App.Core;
using AudioBookPlayer.App.Presenters;
using AudioBookPlayer.Core;
using Xamarin.Essentials;

namespace AudioBookPlayer.App.Views.Activities
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar")]
    [IntentFilter(new [] { Intent.ActionView, Platform.Intent.ActionAppAction }, Categories = new []{ Intent.CategoryDefault })]
    public class MainActivity : AppCompatActivity
    {
        private const int SettingsActivityRequest = 100;

        private MainActivityPresenter? presenter;

        public override void OnRequestPermissionsResult(
            int requestCode,
            string[] permissions,
            [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            PermissionChecker.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        public override bool OnCreateOptionsMenu(IMenu? menu)
        {
            MenuInflater.Inflate(Resource.Menu.main_activity_menu, menu);
            return true;
        }

        public override void OnBackPressed()
        {
            presenter?.OnBackPressed();
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            return (null != presenter && presenter.OnOptionsItemSelected(item)) || base.OnOptionsItemSelected(item);
        }

        protected override void OnCreate(Bundle? savedInstanceState)
        {
            presenter = new MainActivityPresenter();

            base.OnCreate(savedInstanceState);

            Platform.Init(this, savedInstanceState);
            PermissionChecker.Init(this);

            AlbumArt.GetInstance().Initialize();

            presenter.AttachView(this);
        }

        protected override void OnDestroy()
        {
            presenter?.DetachView();
            AlbumArt.GetInstance().Shutdown();

            base.OnDestroy();
        }

        protected override void OnResume()
        {
            base.OnResume();
            Platform.OnResume(this);
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent? data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            if (SettingsActivityRequest == requestCode)
            {
                var response = data?.GetIntExtra("Response", -1) ?? -1;

            }
        }

        protected override void OnNewIntent(Intent? intent)
        {
            base.OnNewIntent(intent);
            Platform.OnNewIntent(intent);
        }

        /*private int NavigateToFragment(AndroidX.Fragment.App.Fragment fragment)
        {
            return SupportFragmentManager
                .BeginTransaction()
                .Replace(Resource.Id.nav_host_frame, fragment)
                .Commit();
        }

        private void StartShareActivity(int test)
        {
            var intent = new Intent(Application.Context, typeof(ShareActivity));
            intent.PutExtra("Test", test);

            var options = new Bundle();
            options.PutString("Option", "test");

            StartActivityForResult(intent, SettingsActivityRequest, options);
        }*/
    }
}

