using AudioBookPlayer.App.Services;
using LibraProgramming.Xamarin.Dependency.Container.Attributes;

namespace AudioBookPlayer.App.ViewModels
{
    internal sealed class RecentBooksViewModel : BooksViewModelBase
    {
        [PrefferedConstructor]
        public RecentBooksViewModel(IBooksProvider books)
            : base(books)
        {
        }

        protected override void DoStartPlay(AudioBookViewModel book)
        {
            base.DoStartPlay(book);
        }

        /*private async void OnRefreshCommand()
        {
            try
            {
                IsBusy = true;

                var status = await permissionRequestor.CheckAndRequestMediaPermissionsAsync();

                if (PermissionStatus.Denied == status)
                {
                    return;
                }

                var path = settings.LibraryRootPath;
                var context = new SourceFolderRequestContext
                {
                    LibraryRootFolder = path
                };

                selectSourceFolder.Raise(context);

                //var temp = await context.Task;
            }
            finally
            {
                IsBusy = false;
            }
        }*/
    }
}
