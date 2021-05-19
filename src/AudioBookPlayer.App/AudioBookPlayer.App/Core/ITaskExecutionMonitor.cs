using System.Threading.Tasks;
using LibraProgramming.Xamarin.Interaction;

namespace AudioBookPlayer.App.Core
{
    public interface ITaskExecutionMonitor
    {
        TaskCompletionSource Source
        {
            get;
        }

        TaskStatus Status
        {
            get;
        }

        void Start();
    }

    public interface ITaskExecutionMonitor<in TValue>
    {
        TaskCompletionSource Source
        {
            get;
        }

        TaskStatus Status
        {
            get;
        }

        void Start(TValue value);
    }
}