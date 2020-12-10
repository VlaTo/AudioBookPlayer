using Android.App;
using Android.OS;
using Android.Runtime;
using Android.Widget;
using LibraProgramming.Xamarin.Popups.Core;
using LibraProgramming.Xamarin.Popups.Platforms.Android;
using LibraProgramming.Xamarin.Popups.Platforms.Xamarin;
using System.Threading.Tasks;
using Xamarin.Forms;

[assembly: Dependency(typeof(AndroidPlatformPopup))]

namespace LibraProgramming.Xamarin.Popups.Platforms.Android
{
    [Preserve(AllMembers = true)]
    internal sealed class AndroidPlatformPopup : IPlatformPopup
    {
        public bool IsInitialized => Popup.IsInitialized;

        public Task AddAsync(PopupContentPage page)
        {
            var view = Popup.GetContentView();

            page.Parent = global::Xamarin.Forms.Application.Current.MainPage;

            var renderer = page.GetOrCreateRenderer();

            if (null != view)
            {
                view.AddView(renderer.View);
            }

            return PostAsync(renderer.View);
        }

        public Task RemoveAsync(PopupContentPage page)
        {
            var renderer = page.GetOrCreateRenderer();

            if (null!=renderer)
            {
                var view = Popup.GetContentView();
                var element = renderer.Element;

                if (null != view)
                {
                    view.RemoveView(renderer.View);
                }

                renderer.Dispose();

                if (null != element)
                {
                    element.Parent = null;
                }

                if (null != view)
                {
                    return PostAsync(view);
                }
            }

            return Task.CompletedTask;
        }

        /*private FrameLayout GetContentView()
        {
            if (Popups.Context is Activity activity)
            {
                if (null != activity.Window && activity.Window.DecorView is FrameLayout layout)
                {
                    return layout;
                }
            }

            return null;
        }*/

        private static Task PostAsync(global::Android.Views.View view)
        {
            if (null == view)
            {
                return Task.CompletedTask;
            }

            var tcs = new TaskCompletionSource<bool>();

            view.Post(() => tcs.SetResult(true));

            return tcs.Task;
        }

        private static bool GetIsSystemAnimationEnabled()
        {
            float animationScale = 0.0f;
            var context = Popup.Context;

            if (null == context)
            {
                return false;
            }

            if (Build.VERSION.SdkInt >= BuildVersionCodes.JellyBeanMr1)
            {
                //animationScale = Settings.Global.GetFloat(
                //    context.ContentResolver,
                //    Settings.Global.AnimatorDurationScale,
                //    1);
            }
            else
            {
                //animationScale = Settings.System.GetFloat(
                //    context.ContentResolver,
                //    Settings.System.AnimatorDurationScale,
                //    1);
            }

            return animationScale > 0.0f;
        }
    }
}
