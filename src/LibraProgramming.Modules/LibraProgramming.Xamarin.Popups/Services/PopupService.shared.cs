using LibraProgramming.Xamarin.Popups.Core;
using LibraProgramming.Xamarin.Popups.Core.Extensions;
using System;
using System.Threading.Tasks;
using LibraProgramming.Xamarin.Popups.Views;
using Xamarin.Forms;

// https://github.com/rotorgames/Rg.Plugins.Popup

namespace LibraProgramming.Xamarin.Popups.Services
{
    public sealed class PopupService : IPopupService
    {
        private IPlatformPopup popup;

        internal IPlatformPopup Popup
        {
            get
            {
                if (null == popup)
                {
                    popup = DependencyService.Get<IPlatformPopup>();

                    if (null == popup)
                    {
                        throw new Exception();
                    }

                    if (false == popup.IsInitialized)
                    {
                        throw new Exception();
                    }
                }

                return popup;
            }
        }

        public Task ShowPopupAsync(PopupContentPage page)
        {
            if (null == page)
            {
                throw new ArgumentNullException(nameof(page));
            }

            var task = InvokeMainThread(async () =>
            {
                await Popup.AddAsync(page);
            });

            return task;
        }

        private static Task InvokeMainThread(Func<Task> func)
        {
            var tcs = new TaskCompletionSource<bool>();

            Device
                .InvokeOnMainThreadAsync(async () =>
                {
                    await func.Invoke();

                    tcs.SetResult(true);
                })
                .RunAndForget();

            return tcs.Task;
        }
    }
}
