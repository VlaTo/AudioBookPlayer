using System;
using System.Threading.Tasks;

namespace LibraProgramming.Xamarin.Interaction
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class Deferral : IDisposable
    {
        public Deferral(TaskCompletionSource tcs)
        {

        }

        public void Complete()
        {

        }

        public void Dispose()
        {
            ;
        }
    }
    
    /// <summary>
    /// 
    /// </summary>
    public sealed class Deferral<T> : IDisposable
    {
        private TaskCompletionSource<T> tcs;
        private bool disposed;

        public bool IsCompleted
        {
            get;
            private set;
        }

        public Deferral(TaskCompletionSource<T> tcs)
        {
            this.tcs = tcs;
        }

        public void Cancel()
        {
            tcs.SetCanceled();
        }

        public void Complete(T value)
        {
            if (tcs.TrySetResult(value) && !IsCompleted)
            {
                IsCompleted = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }

        private void Dispose(bool dispose)
        {
            if (disposed)
            {
                return;
            }

            try
            {
                if (dispose)
                {
                    tcs = null;
                }
            }
            finally
            {
                disposed = true;
            }
        }
    }
}
