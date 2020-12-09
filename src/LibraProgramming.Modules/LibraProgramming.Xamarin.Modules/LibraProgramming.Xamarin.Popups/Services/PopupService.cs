using LibraProgramming.Xamarin.Popups.Core;
using System;
using Xamarin.Forms;

namespace LibraProgramming.Xamarin.Popups.Services
{
    internal sealed class PopupService : IPopupService
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
    }
}
