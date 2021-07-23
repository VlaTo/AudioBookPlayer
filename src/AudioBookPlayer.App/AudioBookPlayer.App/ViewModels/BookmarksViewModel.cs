using System.Collections.ObjectModel;
using System.Threading.Tasks;
using AudioBookPlayer.App.Core;
using AudioBookPlayer.App.Services;
using LibraProgramming.Xamarin.Interaction.Contracts;
using Xamarin.Forms;

namespace AudioBookPlayer.App.ViewModels
{
    public sealed class BookmarksViewModel : ViewModelBase, IInitialize
    {
        private readonly IMediaLibrary mediaLibrary;
        private readonly IPlaybackController playbackController;
        private readonly TaskExecutionMonitor loadBookmarks;

        public ObservableCollection<BookmarkViewModel> Bookmarks
        {
            get;
        }

        public Command RemoveAll
        {
            get;
        }

        public BookmarksViewModel(IMediaLibrary mediaLibrary, IPlaybackController playbackController)
        {
            this.mediaLibrary = mediaLibrary;
            this.playbackController = playbackController;

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
            var bookId = playbackController.AudioBook.Id;
            var bookmarks = await mediaLibrary.QueryBookmarksAsync(bookId.Value);

            foreach (var bookmark in bookmarks)
            {
                var model = new BookmarkViewModel
                {
                    Title = bookmark.Name
                };

                Bookmarks.Add(model);
            }
        }

        private void DoRemoveAll()
        {
            ;
        }
    }
}