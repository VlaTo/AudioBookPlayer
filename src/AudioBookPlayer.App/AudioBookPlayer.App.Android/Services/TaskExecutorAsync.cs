using System;
using System.Threading.Tasks;
using AudioBookPlayer.App.Core.Extensions;

namespace AudioBookPlayer.App.Android.Services
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TEntity"></typeparam>
    internal abstract class TaskExecutorAsync<TKey, TEntity>
    {
        private readonly ITaskLock<TKey, TEntity> taskLock;

        protected TaskExecutorAsync(ITaskLock<TKey, TEntity> taskLock)
        {
            this.taskLock = taskLock;
        }

        public virtual Task<TEntity> ExecuteAsync(TKey key)
        {
            var tcs = new TaskCompletionSource<TEntity>();

            if (taskLock.Acquire(key, tcs, out var current))
            {
                var callback = new Action(() => taskLock.Release(key, current));
                Task.Run(() => DoExecuteAsync(key, current, callback)).RunAndForget();
            }

            return current.Task;
        }

        protected abstract Task DoExecuteAsync(TKey key, TaskCompletionSource<TEntity> tcs, Action callback);
    }
}