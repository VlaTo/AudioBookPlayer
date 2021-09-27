using System.Threading;
using System.Threading.Tasks;
using LibraProgramming.Xamarin.Core;

namespace AudioBookPlayer.App.Android.Services
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    internal sealed class InterlockedTaskLock : ITaskLock
    {
        private TaskCompletionSource completionSource;

        public bool Acquire(TaskCompletionSource comparand, out TaskCompletionSource current)
        {
            var actual = Interlocked.CompareExchange(ref completionSource, comparand, null);

            if (null == actual)
            {
                current = comparand;
                return true;
            }

            current = actual;

            return false;
        }

        public void Release(TaskCompletionSource current)
        {
            Interlocked.CompareExchange(ref completionSource, null, current);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    internal sealed class InterlockedTaskLock<TEntity> : ITaskLock<TEntity>
    {
        private TaskCompletionSource<TEntity> completionSource;

        public bool Acquire(TaskCompletionSource<TEntity> comparand, out TaskCompletionSource<TEntity> current)
        {
            var actual = Interlocked.CompareExchange(ref completionSource, comparand, null);

            if (null == actual)
            {
                current = comparand;
                return true;
            }

            current = actual;

            return false;
        }

        public void Release(TaskCompletionSource<TEntity> current)
        {
            Interlocked.CompareExchange(ref completionSource, null, current);
        }
    }
}