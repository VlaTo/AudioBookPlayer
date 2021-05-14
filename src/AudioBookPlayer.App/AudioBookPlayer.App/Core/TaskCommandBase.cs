using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows.Input;

namespace AudioBookPlayer.App.Core
{
    public abstract class TaskCommandBase : ICommand, INotifyPropertyChanged
    {
        private readonly WeakEventManager eventManager;
        private readonly SynchronizationContext synchronizationContext;
        private readonly Predicate<object> canExecute;
        private bool canBeExecuted;

        public event EventHandler CanExecuteChanged
        {
            add => eventManager.AddEventHandler(value);
            remove => eventManager.RemoveEventHandler(value);
        }

        public event PropertyChangedEventHandler PropertyChanged
        {
            add => eventManager.AddEventHandler(value);
            remove => eventManager.RemoveEventHandler(value);
        }

        public bool CanBeExecuted
        {
            get => canBeExecuted;
            set
            {
                if (value == canBeExecuted)
                {
                    return;
                }

                canBeExecuted = value;

                RaisePropertyChanged();
            }
        }

        public bool IsExecuting
        {
            get;
            private set;
        }

        public ITaskExecutionMonitor ExecutionMonitor
        {
            get;
        }

        protected TaskCommandBase(ITaskExecutionMonitor executionMonitor, Predicate<object> canExecute = null)
        {
            this.canExecute = canExecute;

            eventManager = new WeakEventManager();
            synchronizationContext = SynchronizationContext.Current;
            ExecutionMonitor = executionMonitor;

            if (null != canExecute)
            {
                CanBeExecuted = CanExecute(null);
            }
        }

        public bool CanExecute(object parameter) => canExecute?.Invoke(parameter) ?? false == IsExecuting;

        public async void Execute(object parameter)
        {
            if (IsExecuting)
            {
                return;
            }

            try
            {
                IsExecuting = true;

                RaiseCanExecuteChanged(parameter);

                ExecuteInternal(parameter);

                await ExecutionMonitor.Source.Task;
            }
            finally
            {
                IsExecuting = false;

                RaiseCanExecuteChanged(parameter);
            }
        }

        protected void RaiseCanExecuteChanged(object parameter)
        {
            CanBeExecuted = CanExecute(parameter);

            if (synchronizationContext != null && synchronizationContext != SynchronizationContext.Current)
            {
                synchronizationContext.Post(
                    _ => eventManager.HandleEvent(this, EventArgs.Empty, nameof(CanExecuteChanged)),
                    null
                );
            }
            else
            {
                eventManager.HandleEvent(this, EventArgs.Empty, nameof(CanExecuteChanged));
            }
        }

        protected abstract void ExecuteInternal(object parameter);

        protected void RaisePropertyChanged([CallerMemberName] string propertyName = "")
        {
            eventManager.RaiseEvent(this, new PropertyChangedEventArgs(propertyName), nameof(PropertyChanged));
        }
    }
}