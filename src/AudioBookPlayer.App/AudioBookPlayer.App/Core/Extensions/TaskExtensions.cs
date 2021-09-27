using System;
using System.Threading.Tasks;

namespace AudioBookPlayer.App.Core.Extensions
{
    public static class TaskExtensions
    {
        public static void RunAndForget(this Task task)
        {
            if (null == task)
            {
                throw new ArgumentNullException(nameof(task));
            }

            ;
        }
    }
}