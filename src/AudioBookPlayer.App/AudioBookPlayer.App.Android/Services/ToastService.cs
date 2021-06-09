using Android.App;
using Android.Widget;
using AudioBookPlayer.App.Services;

namespace AudioBookPlayer.App.Android.Services
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class ToastService : IPlatformToastService
    {
        /// <inheritdoc cref="IPlatformToastService.ShowLongMessage" />
        public void ShowLongMessage(string message)
        {
            var toast = Toast.MakeText(Application.Context, message, ToastLength.Long);
            toast.Show();
        }

        /// <inheritdoc cref="IPlatformToastService.ShowShortMessage" />
        public void ShowShortMessage(string message)
        {
            var toast = Toast.MakeText(Application.Context, message, ToastLength.Short);
            var toastView = toast.View;

            toast.Show();
        }
    }
}