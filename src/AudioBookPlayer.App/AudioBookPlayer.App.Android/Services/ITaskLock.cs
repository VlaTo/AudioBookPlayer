using System.Threading.Tasks;
using LibraProgramming.Xamarin.Core;

namespace AudioBookPlayer.App.Android.Services
{
    /// <summary>
    /// 
    /// </summary>
    internal interface ITaskLock
    {
        bool Acquire(TaskCompletionSource comparand, out TaskCompletionSource current);

        void Release(TaskCompletionSource current);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    internal interface ITaskLock<TEntity>
    {
        bool Acquire(TaskCompletionSource<TEntity> comparand, out TaskCompletionSource<TEntity> current);

        void Release(TaskCompletionSource<TEntity> current);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TEntity"></typeparam>
    internal interface ITaskLock<in TKey, TEntity>
    {
        bool Acquire(TKey key, TaskCompletionSource<TEntity> comparand, out TaskCompletionSource<TEntity> current);

        void Release(TKey key, TaskCompletionSource<TEntity> current);
    }
}