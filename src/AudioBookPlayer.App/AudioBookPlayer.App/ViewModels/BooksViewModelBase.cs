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
        private bool isBusy;

        public ObservableCollection<AudioBookViewModel> Books
        {
            get;
        }

        public bool IsBusy
        {
            get => isBusy;
            set => SetProperty(ref isBusy, value);
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
            var token = browserServiceConnector.Library.Subscribe(BindSourceBooks);
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

        protected abstract bool FilterSourceBook(AudioBook source);

        protected virtual void OnConnected()
        {
            //var token = BrowserServiceConnector.Library.Subscribe(BindSourceBooks);
        }

        protected AudioBookViewModel BuildAudioBookModel(AudioBook book)
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

        protected virtual void BindSourceBooks(AudioBook[] audioBooks)
        {
            Books.Clear();

            for (var index = 0; index < audioBooks.Length; index++)
            {
                var source = audioBooks[index];

                if (false == FilterSourceBook(source))
                {
                    continue;
                }

                var model = BuildAudioBookModel(source);

                Books.Add(model);
            }
        }

        private void OnMediaBrowserConnected(Unit _) => OnConnected();
    }
}