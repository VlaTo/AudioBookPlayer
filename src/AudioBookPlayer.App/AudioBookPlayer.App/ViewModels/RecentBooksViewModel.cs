using AudioBookPlayer.App.Domain.Models;
using AudioBookPlayer.App.Services;
using LibraProgramming.Xamarin.Dependency.Container.Attributes;
using System.Collections.Generic;

namespace AudioBookPlayer.App.ViewModels
{
    internal sealed class RecentBooksViewModel : BooksViewModelBase
    {
        [PrefferedConstructor]
        public RecentBooksViewModel(IMediaBrowserServiceConnector browserServiceConnector)
            : base(browserServiceConnector)
        {
        }

        private void DoBindAudioBooks(IEnumerable<AudioBook> audioBooks)
        {
            Books.Clear();

            foreach (var audioBook in audioBooks)
            {
                /*var latest = await MediaLibrary.GetLatestBookActivityAsync(audioBook.Id.Value);
                var temp = DateTime.Now - latest.when;
                if (temp.TotalDays > 7)
                {
                    continue;
                }*/

                var model = CreateAudioBookModel(audioBook);

                Books.Add(model);
            }
        }
    }
}
