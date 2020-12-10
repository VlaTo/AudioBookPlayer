using System;
using System.Threading.Tasks;

namespace LibraProgramming.Xamarin.Popups.Core.Extensions
{
    internal static class TaskExtensions
    {
        public static void RunAndForget(this Task task)
        {
            if (null == task)
            {
                throw new ArgumentNullException(nameof(task));
            }
        }
    }
}
