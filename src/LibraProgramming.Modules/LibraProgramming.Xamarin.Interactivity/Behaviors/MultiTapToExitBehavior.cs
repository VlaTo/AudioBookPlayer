using System;
using Xamarin.Forms;

namespace LibraProgramming.Xamarin.Interactivity.Behaviors
{
    /// <summary>
    /// 
    /// </summary>
    public class MultiTapToExitBehavior : BehaviorBase<Shell>
    {
        /// <summary>
        /// 
        /// </summary>
        public static readonly BindableProperty TapCountProperty;

        /// <summary>
        /// 
        /// </summary>
        public static readonly BindableProperty TimeoutProperty;

        private readonly WeakEventManager eventManager;
        private TimeSpan last;
        private int count;

        /// <summary>
        /// 
        /// </summary>
        public int TapCount
        {
            get => (int) GetValue(TapCountProperty);
            set => SetValue(TapCountProperty, value);
        }

        /// <summary>
        /// 
        /// </summary>
        public TimeSpan Timeout
        {
            get => (TimeSpan) GetValue(TimeoutProperty);
            set => SetValue(TimeoutProperty, value);
        }

        /// <summary>
        /// 
        /// </summary>
        public event EventHandler ShowHintMessage
        {
            add => eventManager.AddEventHandler(value);
            remove => eventManager.RemoveEventHandler(value);
        }

        /// <summary>
        /// 
        /// </summary>
        public MultiTapToExitBehavior()
        {
            eventManager = new WeakEventManager();
            last = TimeSpan.Zero;
        }

        static MultiTapToExitBehavior()
        {
            TimeoutProperty = BindableProperty.Create(
                nameof(Timeout),
                typeof(TimeSpan),
                typeof(MultiTapToExitBehavior),
                TimeSpan.FromSeconds(0.5d)
            );
            TapCountProperty = BindableProperty.Create(
                nameof(TapCount),
                typeof(int),
                typeof(MultiTapToExitBehavior),
                2
            );
        }

        protected override void OnAttachedTo(Shell shell)
        {
            base.OnAttachedTo(shell);

            shell.Navigating += DoShellNavigating;
        }

        private void DoShellNavigating(object sender, ShellNavigatingEventArgs e)
        {
            var shell = (Shell) sender;

            if (ShellNavigationSource.Pop == e.Source)
            {
                if (1 < shell.Navigation.NavigationStack.Count)
                {
                    return;
                }

                if (0 == shell.Navigation.ModalStack.Count && e.CanCancel)
                {
                    var current = TimeSpan.FromMilliseconds(Environment.TickCount);
                    var elapsed = current - last;

                    if (Timeout < elapsed)
                    {
                        count = TapCount;
                    }

                    if (0 == --count)
                    {
                        return;
                    }

                    last = current;
                    eventManager.HandleEvent(this, EventArgs.Empty, nameof(ShowHintMessage));

                    e.Cancel();
                }
            }
        }
    }
}