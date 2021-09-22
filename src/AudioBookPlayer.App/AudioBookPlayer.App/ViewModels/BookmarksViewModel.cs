using AudioBookPlayer.App.Core;
using AudioBookPlayer.App.Services;
using LibraProgramming.Xamarin.Interaction.Contracts;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace AudioBookPlayer.App.ViewModels
{
    public sealed class BookmarksViewModel : ViewModelBase, IInitialize
    {
        private readonly IUnitOfWorkFactory factory;
        private readonly IPlaybackService playbackService;
        private readonly TaskExecutionMonitor loadBookmarks;

        public ObservableCollection<BookmarkViewModel> Bookmarks
        {
            get;
        }

        public Command RemoveAll
        {
            get;
        }

        public BookmarksViewModel(IUnitOfWorkFactory factory, IPlaybackService playbackService)
        {
            this.factory = factory;
            this.playbackService = playbackService;

            loadBookmarks = new TaskExecutionMonitor(LoadBookmarksAsync);
            Bookmarks = new ObservableCollection<BookmarkViewModel>();
            RemoveAll = new Command(DoRemoveAll);
        }

        public void OnInitialize()
        {
            loadBookmarks.Start();
        }

        private async Task LoadBookmarksAsync()
        {
            var bookId = playbackService.AudioBook.Id;

            /*using (var unitOfWork = factory.CreateUnitOfWork(false))
            {
                var bookmarks = await unitOfWork.Bookmarks.QueryAsync(bookId.Value);

                foreach (var bookmark in bookmarks)
                {
                    var model = new BookmarkViewModel
                    {
                        Title = bookmark.Name
                    };

                    Bookmarks.Add(model);
                }
            }*/
        }

        private void DoRemoveAll()
        {
            using (var unitOfWork = factory.CreateUnitOfWork(true))
            {
                /*var bookmarks = await unitOfWork.Bookmarks.QueryAsync(bookId.Value);

                foreach (var bookmark in bookmarks)
                {
                    var model = new BookmarkViewModel
                    {
                        Title = bookmark.Name
                    };

                    Bookmarks.Add(model);
                }*/
            }
        }
    }
}