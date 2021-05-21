using Android.App;
using Android.Widget;
using AudioBookPlayer.App.Services;

namespace AudioBookPlayer.App.Droid.Services
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class ToastService : IToastService
    {
        /// <inheritdoc cref="IToastService.ShowLongMessage" />
        public void ShowLongMessage(string message)
        {
            var toast = Toast.MakeText(Application.Context, message, ToastLength.Long);
            toast.Show();
        }

        /// <inheritdoc cref="IToastService.ShowShortMessage" />
        public void ShowShortMessage(string message)
        {
            var toast = Toast.MakeText(Application.Context, message, ToastLength.Short);
            var toastView = toast.View;

            toast.Show();
        }
    }
}