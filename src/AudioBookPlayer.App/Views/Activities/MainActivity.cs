#nullable enable

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using AndroidX.AppCompat.App;
using AndroidX.AppCompat.Widget;
using AndroidX.Core.View;
using AndroidX.DrawerLayout.Widget;
using AudioBookPlayer.App.Core;
using AudioBookPlayer.App.Views.Fragments;
using Google.Android.Material.FloatingActionButton;
using Google.Android.Material.Navigation;
using Google.Android.Material.Snackbar;
using System;
using System.Reactive.Linq;
using Xamarin.Essentials;

namespace AudioBookPlayer.App.Views.Activities
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar")]
    [IntentFilter(new [] { Intent.ActionView, Platform.Intent.ActionAppAction }, Categories = new []{ Intent.CategoryDefault })]
    public class MainActivity : AppCompatActivity, NavigationView.IOnNavigationItemSelectedListener, View.IOnClickListener
    {
        private const int SettingsActivityRequest = 100;

        private IMenuItem? lastSelectedItem;
        private Toolbar? toolbar;
        private DrawerLayout? drawer;
        private NavigationView? navigationView;
        // private FloatingActionButton? fab;
        private IDisposable? fabClickSubscription;

        internal MediaBrowserServiceConnector? ServiceConnector
        {
            get;
            private set;
        }

        public override void OnRequestPermissionsResult(
            int requestCode,
            string[] permissions,
            [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        public override void OnBackPressed()
        {
            if (null != drawer && drawer.IsDrawerOpen(GravityCompat.Start))
            {
                drawer.CloseDrawer(GravityCompat.Start);
            }
            else
            {
                base.OnBackPressed();
            }
        }

        /*public override bool OnCreateOptionsMenu(IMenu? menu)
        {
            MenuInflater.Inflate(Resource.Menu.menu_main, menu);
            return true;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            var id = item.ItemId;
            
            if (id == Resource.Id.action_settings)
            {
                return true;
            }

            return base.OnOptionsItemSelected(item);
        }*/

        protected override void OnCreate(Bundle? savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            Platform.Init(this, savedInstanceState);
            
            SetContentView(Resource.Layout.activity_main);

            toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);

            if (null != toolbar)
            {
                SetSupportActionBar(toolbar);
                SupportActionBar.SetDisplayHomeAsUpEnabled(true);
                SupportActionBar.SetHomeButtonEnabled(true);
            }

            //fab = FindViewById<FloatingActionButton>(Resource.Id.fab);
            drawer = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
            navigationView = FindViewById<NavigationView>(Resource.Id.nav_view);
            
            /*if (null != fab)
            {
                fabClickSubscription = Observable
                    .FromEventPattern(
                        handler => fab.Click += handler,
                        handler => fab.Click -= handler)
                    .Subscribe(pattern => Snackbar
                        .Make((View?)pattern.Sender, "Replace with your own action", Snackbar.LengthLong)
                        .SetAction("Action", this)
                        .Show()
                    );
            }*/

            if (null != drawer)
            {
                var toggle = new ActionBarDrawerToggle(
                    this,
                    drawer,
                    toolbar,
                    Resource.String.navigation_drawer_open,
                    Resource.String.navigation_drawer_close
                );
                drawer.AddDrawerListener(toggle);
                toggle.SyncState();
            }

            if (null != navigationView)
            {
                navigationView.SetNavigationItemSelectedListener(this);
            }

            AppActions.OnAppAction += OnAppActionsAction;

            if (null == savedInstanceState)
            {
                navigationView?.SetCheckedItem(Resource.Id.nav_library);
                DoNavigation(Resource.Id.nav_library);
            }

            var connector = new MediaBrowserServiceConnector(Application.Context);
            //connector.Connect(this);

            ServiceConnector = connector;
        }

        protected override void OnDestroy()
        {
            ServiceConnector?.Dispose();
            fabClickSubscription?.Dispose();

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

        bool NavigationView.IOnNavigationItemSelectedListener.OnNavigationItemSelected(IMenuItem item)
        {
            if (null != lastSelectedItem)
            {
                lastSelectedItem.SetChecked(false);
            }

            if (null != navigationView)
            {
                navigationView.SetCheckedItem(item.ItemId);
            }

            lastSelectedItem = item;

            DoNavigation(item.ItemId);

            if (null != drawer)
            {
                drawer.CloseDrawer(GravityCompat.Start);
            }

            return true;
        }

        void View.IOnClickListener.OnClick(View? action)
        {
            System.Diagnostics.Debug.WriteLine($"[MainActivity] [OnClick] Action: {action?.Id}");
        }

        private void OnAppActionsAction(object sender, AppActionEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine($"Application action: {e.AppAction.Id}");
        }

        private void DoNavigation(int itemId)
        {
            switch (itemId)
            {
                case Resource.Id.nav_library:
                {
                    var fragment = LibraryFragment.NewInstance();
                    
                    NavigateToFragment(fragment);

                    break;
                }

                case Resource.Id.nav_send:
                {
                    break;
                }

                case Resource.Id.nav_settings:
                {
                    var fragment = SettingsFragment.NewInstance();

                    NavigateToFragment(fragment);

                    break;
                }

                case Resource.Id.nav_share:
                {
                    StartShareActivity(404);

                    break;
                }

                default:
                {
                    break;
                }
            }
        }

        private int NavigateToFragment(AndroidX.Fragment.App.Fragment fragment)
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
        }
    }
}

