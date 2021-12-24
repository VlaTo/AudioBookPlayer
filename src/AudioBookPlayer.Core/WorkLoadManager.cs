#nullable enable

using Android.OS;
using Java.Lang;

namespace AudioBookPlayer.Core
{
    public sealed class WorkLoadManager
    {
        private static WorkLoadManager? instance;
        private WorkLoadThread? thread;

        private WorkLoadManager()
        {
        }

        public static WorkLoadManager Current() => instance ??= new WorkLoadManager();

        public void Enqueue(IRunnable runnable, Bundle? data)
        {
            if (null == thread)
            {
                thread = new WorkLoadThread();
                thread.Start();
                thread.WaitHandle.WaitOne();
            }

            var message = Message.Obtain(thread.Handler, runnable);

            if (null == message)
            { 
                return ;
            }

            if (null != data)
            {
                message.Data = data;
            }

            message.SendToTarget();
        }

        //
        private sealed class WorkLoadThread : Thread
        {
            private readonly System.Threading.ManualResetEvent mre;
            private Looper? looper;

            public Handler? Handler
            {
                get;
                private set;
            }

            public System.Threading.EventWaitHandle WaitHandle => mre;

            public WorkLoadThread()
            {
                looper = null;
                mre = new System.Threading.ManualResetEvent(false);
            }

            public override void Run()
            {
                Looper.Prepare();

                looper = Looper.MyLooper();

                if (null == looper)
                {
                    throw new System.Exception();
                }

                Handler = new Handler(looper);

                mre.Set();

                Looper.Loop();
            }
        }
    }
}