using AudioBookPlayer.App.Domain.Models;
using LibraProgramming.Xamarin.Interaction;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Reactive;
using AudioBookPlayer.App.Services;
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
        private readonly IDisposable subscription;

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

        protected IMediaBrowserServiceConnector BrowserServiceConnector
        {
            get;
        }

        protected BooksViewModelBase(IMediaBrowserServiceConnector browserServiceConnector)
        {
            BrowserServiceConnector = browserServiceConnector;
            Books = new ObservableCollection<AudioBookViewModel>();
            StartPlay = new Command<AudioBookViewModel>(DoStartPlay);
            StartPlayRequest = new InteractionRequest<StartPlayInteractionRequestContext>();

            subscription = browserServiceConnector.Connected.Subscribe(OnMediaBrowserConnected);
        }

        protected virtual void DoStartPlay(AudioBookViewModel book)
        {
            var context = new StartPlayInteractionRequestContext(book.Id);

            StartPlayRequest.Raise(context, () =>
            {
                ;
            });
        }

        protected string GetAuthorsForBook(ICollection<AudioBookAuthor> authors)
        {
            var sep = CultureInfo.CurrentUICulture.TextInfo.ListSeparator;
            return String.Join(sep, authors.Select(author => author.Name));
        }

        protected AudioBookViewModel CreateAudioBookModel(AudioBook book)
        {
            if (false == book.Id.HasValue)
            {
                return null;
            }
            
            var model = new AudioBookViewModel
            {
                Id = book.Id.Value,
                Title = book.Title,
                Authors = GetAuthorsForBook(book.Authors),
                Synopsis = book.Synopsis,
                Duration = book.Duration
            };

            if (0 < book.Images.Count)
            {
                model.ImageSource = book.Images[0].GetStreamAsync;
            }

            return model;
        }

        protected virtual void OnConnected()
        {
            System.Diagnostics.Debug.WriteLine("[BooksViewModelBase] [OnConnected] Execute");
        }

        private void OnMediaBrowserConnected(Unit _) => OnConnected();
    }
}