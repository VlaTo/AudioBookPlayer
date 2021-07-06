using AudioBookPlayer.App.Services;
using LibraProgramming.Xamarin.Interaction;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using AudioBookPlayer.App.Core;
using AudioBookPlayer.App.Extensions;
using AudioBookPlayer.App.Models;
using LibraProgramming.Media.Common;
using Xamarin.Forms;

namespace AudioBookPlayer.App.ViewModels
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class PlayBookInteractionRequestContext : InteractionRequestContext
    {
        public long BookId
        {
            get;
        }

        public PlayBookInteractionRequestContext(long bookId)
        {
            BookId = bookId;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    internal abstract class BooksViewModelBase : ViewModelBase, IBooksViewModel
    {
        protected readonly IBookShelfProvider BookShelf;
        private readonly TaskExecutionMonitor<AudioBook[]> executionMonitor;

        public ObservableCollection<AudioBookViewModel> Books
        {
            get;
        }

        public InteractionRequest<PlayBookInteractionRequestContext> PlayBookRequest
        {
            get;
        }

        public Command<AudioBookViewModel> PlayBook
        {
            get;
        }

        protected BooksViewModelBase(IBookShelfProvider bookShelf)
        {
            executionMonitor = new TaskExecutionMonitor<AudioBook[]>(BindBooks);

            BookShelf = bookShelf;
            Books = new ObservableCollection<AudioBookViewModel>();
            PlayBook = new Command<AudioBookViewModel>(DoPlayBook);
            PlayBookRequest = new InteractionRequest<PlayBookInteractionRequestContext>();

            bookShelf.QueryBooksReady += OnQueryBooksReady;
        }

        protected virtual void DoPlayBook(AudioBookViewModel book)
        {
            var context = new PlayBookInteractionRequestContext(book.Id)
            {

            };

            PlayBookRequest.Raise(context, () =>
            {
                ;
            });
        }

        protected virtual void OnQueryBooksReady(object sender, AudioBooksEventArgs e)
        {
            executionMonitor.Start(e.Books);
            
            /*Books.Clear();

            foreach (var book in e.Books)
            {
                Books.Add(new AudioBookViewModel
                {
                    Id = book.Id.GetValueOrDefault(-1L),
                    Title = book.Title,
                    Authors = GetAuthorsForBook(book.Authors),
                    Synopsis = book.Synopsis,
                    Duration = book.Duration,
                    ImageBlob = await book.GetImageAsync(WellKnownMetaItemNames.Cover)
                });
            }*/
        }

        protected string GetAuthorsForBook(ICollection<AudioBookAuthor> authors)
        {
            var sep = CultureInfo.CurrentUICulture.TextInfo.ListSeparator;
            return String.Join(sep, authors.Select(author => author.Name));
        }

        protected void AddBookToList(AudioBook audioBook)
        {
            Books.Add(new AudioBookViewModel
            {
                Id = audioBook.Id.GetValueOrDefault(-1L),
                Title = audioBook.Title,
                Authors = GetAuthorsForBook(audioBook.Authors),
                Synopsis = audioBook.Synopsis,
                Duration = audioBook.Duration,
                //ImageSource = await audioBook.GetImageAsync(WellKnownMetaItemNames.Cover)
            });
        }

        protected virtual Task BindBooks(AudioBook[] audioBooks)
        {
            return Task.CompletedTask;
        }
    }
}