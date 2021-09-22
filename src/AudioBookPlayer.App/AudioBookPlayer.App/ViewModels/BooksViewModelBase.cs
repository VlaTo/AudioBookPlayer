using AudioBookPlayer.App.Services;
using LibraProgramming.Xamarin.Interaction;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Threading.Tasks;
using AudioBookPlayer.App.Core;
using AudioBookPlayer.App.Domain.Models;
using AudioBookPlayer.App.Domain.Services;
using LibraProgramming.Xamarin.Interaction.Contracts;
using Xamarin.Forms;

namespace AudioBookPlayer.App.ViewModels
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class StartPlayInteractionRequestContext : InteractionRequestContext
    {
        public BookId BookId
        {
            get;
        }

        public StartPlayInteractionRequestContext(BookId bookId)
        {
            BookId = bookId;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    internal abstract class BooksViewModelBase : ViewModelBase, IInitialize, IBooksViewModel
    {
        private readonly IDisposable subscription;
        private readonly TaskExecutionMonitor loadBooksExecution;
        private bool isBusy;

        public ObservableCollection<BookPreviewViewModel> Books
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

        public Command<BookPreviewViewModel> StartPlay
        {
            get;
        }

        protected IMediaBrowserServiceConnector BrowserServiceConnector
        {
            get;
        }

        protected ICoverService CoverService
        {
            get;
        }

        protected BooksViewModelBase(
            IMediaBrowserServiceConnector browserServiceConnector,
            ICoverService coverService)
        {
            BrowserServiceConnector = browserServiceConnector;
            CoverService = coverService;
            Books = new ObservableCollection<BookPreviewViewModel>();
            StartPlay = new Command<BookPreviewViewModel>(DoStartPlay);
            StartPlayRequest = new InteractionRequest<StartPlayInteractionRequestContext>();

            loadBooksExecution = new TaskExecutionMonitor(DoLoadBooksAsync);

            //subscription = browserServiceConnector.Connected.Subscribe(OnMediaBrowserConnected);
            //var token = browserServiceConnector.Library.Subscribe(BindSourceBooks);
        }

        public virtual void OnInitialize()
        {
            loadBooksExecution.Start();
        }

        protected virtual void DoStartPlay(BookPreviewViewModel book)
        {
            var context = new StartPlayInteractionRequestContext(book.Id);

            StartPlayRequest.Raise(context, () =>
            {
                ;
            });
        }

        protected abstract bool FilterSourceBook(BookPreviewViewModel model);

        protected virtual void OnConnected()
        {
            //var token = BrowserServiceConnector.Library.Subscribe(BindSourceBooks);
        }

        /*protected AudioBookViewModel BuildAudioBookModel(AudioBook book)
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
        }*/


        protected virtual void BindSourceBooks(IReadOnlyList<BookPreviewViewModel> models)
        {
            Books.Clear();

            for (var index = 0; index < models.Count; index++)
            {
                var model = models[index];

                if (false == FilterSourceBook(model))
                {
                    continue;
                }

                Books.Add(model);
            }
        }

        private async Task DoLoadBooksAsync()
        {
            await BrowserServiceConnector.ConnectAsync();

            var library = await BrowserServiceConnector.GetLibraryAsync(CoverService);

            BindSourceBooks(library);
        }

        private void OnMediaBrowserConnected(Unit _) => OnConnected();
    }
}