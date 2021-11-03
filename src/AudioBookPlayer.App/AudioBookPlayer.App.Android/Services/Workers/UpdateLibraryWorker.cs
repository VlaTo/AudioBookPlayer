#nullable enable

using System;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using AndroidX.Concurrent.Futures;
using AndroidX.Work;
using Google.Common.Util.Concurrent;
using Object = Java.Lang.Object;
using Thread = System.Threading.Thread;

namespace AudioBookPlayer.App.Android.Services.Workers
{
    internal sealed class UpdateLibraryWorker : ListenableWorker, CallbackToFutureAdapter.IResolver
    {
        //private readonly IBooksProvider booksProvider;
        //private readonly IBooksService booksService;

        public UpdateLibraryWorker(Context context, WorkerParameters workerParams)
            : base(context, workerParams)
        {
            var notificationService = (NotificationManager?)context.GetSystemService(Context.NotificationService);

            //booksService = AudioBookPlayerApplication.Instance.DependencyContainer.GetInstance<IBooksService>();
            //booksProvider = AudioBookPlayerApplication.Instance.DependencyContainer.GetInstance<IBooksProvider>();
        }

        public override IListenableFuture StartWork() => CallbackToFutureAdapter.GetFuture(this);

        Object CallbackToFutureAdapter.IResolver.AttachCompleter(CallbackToFutureAdapter.Completer p0)
        {
            p0.AddCancellationListener(null, BackgroundExecutor);

            Task.Run(() =>
            {
                for (int pass = 0; pass < 6; pass++)
                {
                    var result = new Data.Builder()
                        .PutInt("Overall", 5)
                        .PutInt("Progress", pass)
                        .Build();

                    SetProgressAsync(result);

                    Thread.Sleep(TimeSpan.FromSeconds(5.0d));
                }
                
                p0.Set(Result.InvokeSuccess());
            });

            return new Object();
        }

        /*public override Result DoWork()
        {
            var result = new Data.Builder()
                .PutInt("Test", 42)
                .Build();

            var booksLibrary = new AudioBooksLibrary();

            // 1. Get books from device and library
            var actualBooks = booksProvider.QueryBooks();
            var libraryBooks = booksService.QueryBooks();
            // 2. Compare collections, get differences
            var changes = booksLibrary.GetChanges(libraryBooks, actualBooks);
            // 3. Apply differences to library
            if (0 < changes.Count)
            {
                var success = await booksLibrary.TryApplyChangesAsync(booksService, changes, CancellationToken.None);

                if (success)
                {
                    result = new Bundle();

                    result.PutInt("Count", changes.Count);
                }
            }

            return Result.InvokeSuccess(result);
        }*/
    }
}