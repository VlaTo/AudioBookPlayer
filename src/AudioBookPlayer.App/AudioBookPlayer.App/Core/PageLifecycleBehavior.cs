using System;
using Xamarin.Forms;

namespace AudioBookPlayer.App.Core
{
    internal sealed class PageLifecycleBehavior : BehaviorBase<Page>
    {
        protected override void OnAttachedTo(Page bindable)
        {
            base.OnAttachedTo(bindable);

            bindable.Appearing += DoPageAppearing;
            bindable.Disappearing += DoPageDisappearing;
        }

        private void DoPageAppearing(object sender, EventArgs e)
        {
            PageInvocation.InvokeViewModelAction<IInitialize>(AttachedObject, aware => aware.OnInitialize());
        }

        private void DoPageDisappearing(object sender, EventArgs e)
        {
            PageInvocation.InvokeViewModelAction<IDestructible>(AttachedObject, aware => aware.OnDestroy());
        }
    }
}
