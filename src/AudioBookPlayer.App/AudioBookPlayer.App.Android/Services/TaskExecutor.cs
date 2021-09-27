using System;
using System.Threading.Tasks;
using LibraProgramming.Xamarin.Core;

namespace AudioBookPlayer.App.Android.Services
{
    /// <summary>
    /// 
    /// </summary>
    internal abstract class TaskExecutor
    {
        private readonly ITaskLock taskLock;

        protected TaskExecutor(ITaskLock taskLock)
        {
            this.taskLock = taskLock;
        }

        public virtual Task ExecuteAsync()
        {
            var tcs = new TaskCompletionSource();

            if (taskLock.Acquire(tcs, out var current))
            {
                var callback = new Action(() => taskLock.Release(current));
                DoExecute(current, callback);
            }

            return current.Task;
        }

        protected abstract void DoExecute(TaskCompletionSource tcs, Action callback);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    internal abstract class TaskExecutor<TEntity>
    {
        private readonly ITaskLock<TEntity> taskLock;

        protected TaskExecutor(ITaskLock<TEntity> taskLock)
        {
            this.taskLock = taskLock;
        }

        public virtual Task<TEntity> ExecuteAsync()
        {
            var tcs = new TaskCompletionSource<TEntity>();

            if (taskLock.Acquire(tcs, out var current))
            {
                var callback = new Action(() => taskLock.Release(current));
                DoExecute(current, callback);
            }

            return current.Task;
        }

        protected abstract void DoExecute(TaskCompletionSource<TEntity> tcs, Action callback);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TEntity"></typeparam>
    internal abstract class TaskExecutor<TKey, TEntity>
    {
        private readonly ITaskLock<TKey, TEntity> taskLock;

        protected TaskExecutor(ITaskLock<TKey, TEntity> taskLock)
        {
            this.taskLock = taskLock;
        }

        public virtual Task<TEntity> ExecuteAsync(TKey key)
        {
            var tcs = new TaskCompletionSource<TEntity>();

            if (taskLock.Acquire(key, tcs, out var current))
            {
                var callback = new Action(() => taskLock.Release(key, current));
                DoExecute(key, current, callback);
            }

            return current.Task;
        }

        protected abstract void DoExecute(TKey key, TaskCompletionSource<TEntity> tcs, Action callback);
    }
}