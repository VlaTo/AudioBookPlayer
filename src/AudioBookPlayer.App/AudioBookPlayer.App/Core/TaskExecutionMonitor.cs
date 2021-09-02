using System;
using System.Threading.Tasks;

namespace AudioBookPlayer.App.Core
{
    public class TaskExecutionMonitor : TaskExecutionMonitorBase
    {
        public TaskExecutionMonitor(Func<Task> source)
            : base(source)
        {
        }
    }
    
    public class TaskExecutionMonitor<TValue> : TaskExecutionMonitorBase<TValue>
    {
        public TaskExecutionMonitor(Func<TValue, Task> source)
            : base(source)
        {
        }
    }
}