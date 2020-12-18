using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LibraProgramming.Xamarin.Interaction
{
    public sealed class TaskCompletionSource
    {
        private readonly TaskCompletionSource<object> tcs;

        public Task Task => tcs.Task;

        public TaskCompletionSource()
        {
            tcs = new TaskCompletionSource<object>();
        }

        public TaskCompletionSource(object state)
        {
            tcs = new TaskCompletionSource<object>(state);
        }

        public TaskCompletionSource(TaskCreationOptions options)
        {
            tcs = new TaskCompletionSource<object>(options);
        }

        public TaskCompletionSource(object state, TaskCreationOptions options)
        {
            tcs = new TaskCompletionSource<object>(state, options);
        }

        public void Cancel() => tcs.SetCanceled();

        public bool TryCancel() => tcs.TrySetCanceled();

        public void Fail(Exception exception) => tcs.SetException(exception);

        public void Fail(IEnumerable<Exception> exceptions) => tcs.SetException(exceptions);

        public bool TryFail(Exception exception) => tcs.TrySetException(exception);
        
        public bool TryFail(IEnumerable<Exception> exceptions) => tcs.TrySetException(exceptions);

        public void Complete() => tcs.SetResult(null);

        public bool TryComplete() => tcs.TrySetResult(null);
    }
}
