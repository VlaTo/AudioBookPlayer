using System;
using System.Threading.Tasks;

namespace AudioBookPlayer.App.Core.Extensions
{
    internal static class TaskExtensions
    {
        public static void FireAndForget(this Task task)
        {
            if (null == task)
            {
                throw new ArgumentNullException(nameof(task));
            }

            ;
        }
    }
}