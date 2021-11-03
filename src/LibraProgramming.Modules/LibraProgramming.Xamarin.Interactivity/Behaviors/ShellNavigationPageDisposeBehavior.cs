using System;
using LibraProgramming.Xamarin.Interaction;
using LibraProgramming.Xamarin.Interaction.Contracts;
using Xamarin.Forms;

namespace LibraProgramming.Xamarin.Interactivity.Behaviors
{
    public sealed class ShellNavigationPageDisposeBehavior : BehaviorBase<Shell>
    {
        private Page[] pagesToLeave;

        public ShellNavigationPageDisposeBehavior()
        {
            pagesToLeave = Array.Empty<Page>();
        }

        protected override void OnAttachedTo(Shell shell)
        {
            base.OnAttachedTo(shell);

            shell.Navigating += DoShellNavigating;
            shell.Navigated += DoShellNavigated;
        }

        protected override void OnDetachingFrom(Shell shell)
        {
            base.OnDetachingFrom(shell);

            shell.Navigating -= DoShellNavigating;
            shell.Navigated -= DoShellNavigated;
        }

        private void DoShellNavigating(object sender, ShellNavigatingEventArgs e)
        {
            var shell = (Shell)sender;

            if (ShellNavigationSource.Pop == e.Source)
            {
                var stackCount = shell.Navigation.NavigationStack.Count;

                if (1 < stackCount && false == e.Cancelled)
                {
                    var page = shell.Navigation.NavigationStack[stackCount - 1];
                    pagesToLeave = new[] { page };
                }

                return;
            }

            if (ShellNavigationSource.PopToRoot == e.Source)
            {
                var stackCount = shell.Navigation.NavigationStack.Count;

                if (false == e.Cancelled)
                {
                    var pages = new Page[stackCount - 1];

                    for (var index = stackCount - 1; index > 0; index--)
                    {
                        var page = shell.Navigation.NavigationStack[index];
                        pages[stackCount - index - 1] = page;
                    }

                    pagesToLeave = pages;
                }
            }
        }

        private void DoShellNavigated(object sender, ShellNavigatedEventArgs e)
        {
            for (var index = 0; index < pagesToLeave.Length; index++)
            {
                var page = pagesToLeave[index];
                PageInvocation.InvokeViewModelAction<ICleanup>(page, aware => aware.OnCleanup());
            }

            pagesToLeave = Array.Empty<Page>();
        }
    }
}