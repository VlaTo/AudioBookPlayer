using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LibraProgramming.Xamarin.Core
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class TaskCompletionSource
    {
        private readonly TaskCompletionSource<object> tcs;

        /// <summary>
        /// 
        /// </summary>
        public Task Task => tcs.Task;

        /// <summary>
        /// 
        /// </summary>
        public TaskCompletionSource()
        {
            tcs = new TaskCompletionSource<object>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="state"></param>
        public TaskCompletionSource(object state)
        {
            tcs = new TaskCompletionSource<object>(state);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="options"></param>
        public TaskCompletionSource(TaskCreationOptions options)
        {
            tcs = new TaskCompletionSource<object>(options);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="state"></param>
        /// <param name="options"></param>
        public TaskCompletionSource(object state, TaskCreationOptions options)
        {
            tcs = new TaskCompletionSource<object>(state, options);
        }

        /// <summary>
        /// 
        /// </summary>
        public void Cancel() => tcs.SetCanceled();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool TryCancel() => tcs.TrySetCanceled();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="exception"></param>
        public void Fail(Exception exception) => tcs.SetException(exception);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="exceptions"></param>
        public void Fail(IEnumerable<Exception> exceptions) => tcs.SetException(exceptions);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="exception"></param>
        /// <returns></returns>
        public bool TryFail(Exception exception) => tcs.TrySetException(exception);
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="exceptions"></param>
        /// <returns></returns>
        public bool TryFail(IEnumerable<Exception> exceptions) => tcs.TrySetException(exceptions);

        /// <summary>
        /// 
        /// </summary>
        public void Complete() => tcs.SetResult(null);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool TryComplete() => tcs.TrySetResult(null);
    }
}
