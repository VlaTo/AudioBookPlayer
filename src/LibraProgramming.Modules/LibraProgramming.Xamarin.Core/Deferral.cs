using System;
using System.Threading.Tasks;

namespace LibraProgramming.Xamarin.Core
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class Deferral : IDisposable
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="tcs"></param>
        public Deferral(TaskCompletionSource tcs)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        public void Complete()
        {

        }

        /// <summary>
        /// 
        /// </summary>
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

        /// <summary>
        /// 
        /// </summary>
        public bool IsCompleted
        {
            get;
            private set;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tcs"></param>
        public Deferral(TaskCompletionSource<T> tcs)
        {
            this.tcs = tcs;
        }

        /// <summary>
        /// 
        /// </summary>
        public void Cancel()
        {
            tcs.SetCanceled();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        public void Complete(T value)
        {
            if (tcs.TrySetResult(value) && !IsCompleted)
            {
                IsCompleted = true;
            }
        }

        /// <summary>
        /// 
        /// </summary>
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
