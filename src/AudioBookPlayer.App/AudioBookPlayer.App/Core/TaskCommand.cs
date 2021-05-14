using System;
using System.Threading.Tasks;

namespace AudioBookPlayer.App.Core
{
    public class TaskCommand : TaskCommandBase
    {
        private readonly Func<Task> action;

        public TaskCommand(Func<Task> action, Predicate<object> canExecute = null)
            : base(new TaskExecutionMonitor(action), canExecute)
        {
            this.action = action;
        }

        protected override void ExecuteInternal(object parameter)
        {
            ExecutionMonitor.Start();
        }
    }
}