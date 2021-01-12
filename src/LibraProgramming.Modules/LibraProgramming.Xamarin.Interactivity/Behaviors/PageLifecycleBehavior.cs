using LibraProgramming.Xamarin.Interaction;
using LibraProgramming.Xamarin.Interaction.Contracts;
using System;
using Xamarin.Forms;

namespace LibraProgramming.Xamarin.Interactivity.Behaviors
{
    public sealed class PageLifecycleBehavior : BehaviorBase<Page>
    {
        protected override void OnAttachedTo(Page page)
        {
            base.OnAttachedTo(page);

            page.Appearing += DoPageAppearing;
            page.Disappearing += DoPageDisappearing;

            PageInvocation.InvokeViewModelAction<IInitialize>(AttachedObject, aware => aware.OnInitialize());
            //await PageInvocation.InvokeViewModelActionAsync<IInitializeAsync>(AttachedObject, aware => aware.OnInitialize());
        }

        private void DoPageAppearing(object sender, EventArgs e)
        {
            PageInvocation.InvokeViewModelAction<IPageLifecycleAware>(AttachedObject, aware => aware.OnAppearing());
        }

        private void DoPageDisappearing(object sender, EventArgs e)
        {
            PageInvocation.InvokeViewModelAction<IPageLifecycleAware>(AttachedObject, aware => aware.OnDisappearing());
        }
    }
}
