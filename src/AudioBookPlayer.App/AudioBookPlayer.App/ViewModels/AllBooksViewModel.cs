using System;
using System.Collections.Generic;
using AudioBookPlayer.App.Services;
using LibraProgramming.Xamarin.Dependency.Container.Attributes;
using System.Collections.ObjectModel;
using System.Globalization;
using Xamarin.Forms;

namespace AudioBookPlayer.App.ViewModels
{
    public sealed class AllBooksViewModel : ViewModelBase
    {
        private readonly IBookShelfProvider bookShelf;

        public ObservableCollection<AudioBookViewModel> Books
        {
            get;
        }

        public Command<AudioBookViewModel> PlayBook
        {
            get;
        }

        [PrefferedConstructor]
        public AllBooksViewModel(IBookShelfProvider bookShelf)
        {
            this.bookShelf = bookShelf;

            Books = new ObservableCollection<AudioBookViewModel>();
            PlayBook = new Command<AudioBookViewModel>(DoPlayBook);

            bookShelf.QueryBooksReady += OnQueryBooksReady;
        }

        private void DoPlayBook(AudioBookViewModel book)
        {
            ;
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
                    Title = book.Title,
                    Authors = GetAuthorsForBook(book.Authors),
                    Synopsis = book.Synopsis,
                    Duration = book.Duration
                });
            }
        }

        private string GetAuthorsForBook(IList<string> authors)
        {
            var sep = CultureInfo.CurrentUICulture.TextInfo.ListSeparator;
            return String.Join(sep, authors);
        }
    }
}
