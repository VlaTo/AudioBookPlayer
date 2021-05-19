using System;
using System.Threading.Tasks;
using LibraProgramming.Xamarin.Interaction;

namespace AudioBookPlayer.App.Core
{
    public abstract class TaskExecutionMonitorBase : ITaskExecutionMonitor
    {
        private readonly Func<Task> source;
        private Task task;

        public TaskCompletionSource Source
        {
            get;
        }

        public TaskStatus Status
        {
            get;
            private set;
        }

        protected TaskExecutionMonitorBase(
            Func<Task> source
        )
        {
            this.source = source;

            Source = new TaskCompletionSource();
            Status = TaskStatus.WaitingForActivation;
        }

        public void Start()
        {
            task = MonitorSourceTaskAsync();
        }

        protected async Task MonitorSourceTaskAsync()
        {
            Status = TaskStatus.Running;

            try
            {

                await Task.Run(source);

            }
            catch (TaskCanceledException)
            {
                Source.TryCancel();
                Status = TaskStatus.Canceled;
            }
            catch (Exception exception)
            {
                Source.TryFail(exception);
                Status = TaskStatus.Faulted;
            }
            finally
            {
                Source.TryComplete();
                Status = TaskStatus.RanToCompletion;
            }
        }
    }
    
    public abstract class TaskExecutionMonitorBase<TValue> : ITaskExecutionMonitor<TValue>
    {
        private readonly Func<TValue, Task> source;
        private TaskStatus status;
        private Task task;

        public TaskCompletionSource Source { get; }

        public TaskStatus Status => status;

        protected TaskExecutionMonitorBase(
            Func<TValue, Task> source
        )
        {
            this.source = source;
            status = TaskStatus.WaitingForActivation;

            Source = new TaskCompletionSource();
        }

        public void Start(TValue value)
        {
            task = MonitorSourceTaskAsync(value);
        }

        protected async Task MonitorSourceTaskAsync(TValue value)
        {
            status = TaskStatus.Running;

            try
            {

                await Task.Run(() => source.Invoke(value));

            }
            catch (TaskCanceledException)
            {
                Source.TryCancel();
                status = TaskStatus.Canceled;
            }
            catch (Exception exception)
            {
                Source.TryFail(exception);
                status = TaskStatus.Faulted;
            }
            finally
            {
                Source.TryComplete();
                status = TaskStatus.RanToCompletion;
            }
        }
    }
}