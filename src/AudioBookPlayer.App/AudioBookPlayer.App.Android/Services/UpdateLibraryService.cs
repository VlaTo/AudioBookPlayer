using Android.App;
using Android.Runtime;
using AndroidX.Lifecycle;
using AndroidX.Work;
using AudioBookPlayer.App.Android.Services.Workers;
using AudioBookPlayer.App.Services;
using Java.Lang;
using Xamarin.Essentials;

namespace AudioBookPlayer.App.Android.Services
{
    internal sealed class UpdateLibraryService : Object, IUpdateLibraryService
    {
        private readonly WorkManager workManager;
        private IOperation current;

        public UpdateLibraryService()
        {
            workManager = WorkManager.GetInstance(Application.Context);
        }

        public void StartUpdate()
        {
            var request = new OneTimeWorkRequest.Builder(typeof(UpdateLibraryWorker))
                .SetInputData(Data.Empty)
                .Build();

            current = workManager.Enqueue(request);

            var observer = new WorkObserver();
            var temp = (MainActivity)Platform.CurrentActivity;

            current.State.Observe(temp, observer);
        }

        private sealed class WorkObserver : Object, IObserver
        {
            public WorkObserver()
            {
            }

            public void OnChanged(Object p0)
            {
                var state = p0.JavaCast<OperationState>();
                System.Diagnostics.Debug.WriteLine($"[WorkObserver] [OnChanged] State: {state}");

                /*var overall = workInfo.Progress.GetInt("Overall", 0);
                var progress = workInfo.Progress.GetInt("Progress", 0);
                var state = workInfo.GetState();

                if (WorkInfo.State.Running == state)
                {
                    System.Diagnostics.Debug.WriteLine($"[WorkObserver] [OnChanged] {progress}/{overall}");
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"[WorkObserver] [OnChanged] State: {state}");
                }*/
            }
        }
    }
}