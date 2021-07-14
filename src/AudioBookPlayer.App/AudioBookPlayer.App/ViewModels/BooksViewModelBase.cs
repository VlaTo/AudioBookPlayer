using AudioBookPlayer.App.Models;
using AudioBookPlayer.App.Services;
using LibraProgramming.Xamarin.Interaction;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using Xamarin.Forms;

namespace AudioBookPlayer.App.ViewModels
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class StartPlayInteractionRequestContext : InteractionRequestContext
    {
        public long BookId
        {
            get;
        }

        public StartPlayInteractionRequestContext(long bookId)
        {
            BookId = bookId;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    internal abstract class BooksViewModelBase : ViewModelBase, IBooksViewModel
    {
        public ObservableCollection<AudioBookViewModel> Books
        {
            get;
        }

        public InteractionRequest<StartPlayInteractionRequestContext> StartPlayRequest
        {
            get;
        }

        public Command<AudioBookViewModel> StartPlay
        {
            get;
        }

        protected readonly IBooksProvider Provider;

        protected BooksViewModelBase(IBooksProvider provider)
        {
            Provider = provider;
            Books = new ObservableCollection<AudioBookViewModel>();
            StartPlay = new Command<AudioBookViewModel>(DoStartPlay);
            StartPlayRequest = new InteractionRequest<StartPlayInteractionRequestContext>();
        }

        protected virtual void DoStartPlay(AudioBookViewModel book)
        {
            var context = new StartPlayInteractionRequestContext(book.Id);

            StartPlayRequest.Raise(context, () =>
            {
                ;
            });
        }

        /*protected virtual void OnQueryBooksReady(object sender, AudioBooksEventArgs e)
        {
            executionMonitor.Start(e.Books);
            
            Books.Clear();

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
            }
        }*/

        protected string GetAuthorsForBook(ICollection<AudioBookAuthor> authors)
        {
            var sep = CultureInfo.CurrentUICulture.TextInfo.ListSeparator;
            return String.Join(sep, authors.Select(author => author.Name));
        }

        protected void AddBookToList(AudioBook audioBook)
        {
            Books.Add(new AudioBookViewModel
            {
                Id = audioBook.Id.Value,
                Title = audioBook.Title,
                Authors = GetAuthorsForBook(audioBook.Authors),
                Synopsis = audioBook.Synopsis,
                Duration = audioBook.Duration,
                //ImageSource = await audioBook.GetImageAsync(WellKnownMetaItemNames.Cover)
            });
        }
    }
}