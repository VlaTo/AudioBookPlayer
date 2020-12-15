using LibraProgramming.Xamarin.Interaction.Contracts;
using System;
using Xamarin.Forms;

namespace LibraProgramming.Xamarin.Interaction
{
    public sealed class WeakLifetimeObjectObserver
    {
        private readonly WeakReference<ILifetimeTarget> target;

        public WeakLifetimeObjectObserver(Page page, ILifetimeTarget target)
        {
            this.target = new WeakReference<ILifetimeTarget>(target);

            page.Appearing += OnPageAppearing;
            page.Disappearing += OnPageDisappearing;
        }

        private void OnPageAppearing(object sender, EventArgs e)
        {
            var page = sender as Page;

            ILifetimeTarget lifetime;

            if (target.TryGetTarget(out lifetime) && null != page)
            {
                lifetime.OnAppearing(page);
            }
        }

        private void OnPageDisappearing(object sender, EventArgs e)
        {
            var page = sender as Page;

            ILifetimeTarget lifetime;

            if (target.TryGetTarget(out lifetime) && null != page)
            {
                lifetime.OnDisappearing(page);
            }
        }
    }
}
