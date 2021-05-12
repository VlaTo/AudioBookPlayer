using AudioBookPlayer.App.Services;
using LibraProgramming.Xamarin.Dependency.Container.Attributes;
using System.Collections.ObjectModel;

namespace AudioBookPlayer.App.ViewModels
{
    public sealed class AllBooksViewModel : ViewModelBase
    {
        private readonly IBookShelfProvider bookShelf;

        public ObservableCollection<AudioBookViewModel> Books
        {
            get;
        }

        [PrefferedConstructor]
        public AllBooksViewModel(IBookShelfProvider bookShelf)
        {
            this.bookShelf = bookShelf;

            Books = new ObservableCollection<AudioBookViewModel>();

            bookShelf.QueryBooksReady += OnQueryBooksReady;
        }

        /*void IInitialize.OnInitialize()
        {
            System.Diagnostics.Debug.WriteLine($"[AllBooksViewModel] [OnInitialize] Executed");
        }*/

        private void OnQueryBooksReady(object sender, AudioBooksEventArgs e)
        {
            Books.Clear();

            foreach (var book in e.Books)
            {
                Books.Add(new AudioBookViewModel
                {
                    Title = book.Title
                });
            }
        }
    }
}
