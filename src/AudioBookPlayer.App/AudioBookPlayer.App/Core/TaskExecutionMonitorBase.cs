using System;
using System.Threading.Tasks;
using LibraProgramming.Xamarin.Interaction;

namespace AudioBookPlayer.App.Core
{
    public abstract class TaskExecutionMonitorBase : ITaskExecutionMonitor
    {
        private readonly Func<Task> source;
        private Task task;

        public TaskCompletionSource Source { get; }
        
        public TaskStatus Status { get; }

        protected TaskExecutionMonitorBase(
            Func<Task> source
        )
        {
            this.source = source;

            Source = new TaskCompletionSource();
        }

        public void Start()
        {
            task = MonitorSourceTaskAsync();
        }

        protected async Task MonitorSourceTaskAsync()
        {
            try
            {

                await Task.Run(source);

            }
            catch (TaskCanceledException)
            {
                Source.TryCancel();
            }
            catch (Exception exception)
            {
                Source.TryFail(exception);
            }
            finally
            {
                Source.TryComplete();
            }
        }
    }
}