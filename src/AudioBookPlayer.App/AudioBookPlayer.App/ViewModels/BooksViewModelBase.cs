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
        public EntityId EntityId
        {
            get;
        }

        public StartPlayInteractionRequestContext(EntityId entityId)
        {
            EntityId = entityId;
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

        public ObservableCollection<BookItemViewModel> Books
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

        public Command<BookItemViewModel> StartPlay
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
            Books = new ObservableCollection<BookItemViewModel>();
            StartPlay = new Command<BookItemViewModel>(DoStartPlay);
            StartPlayRequest = new InteractionRequest<StartPlayInteractionRequestContext>();

            loadBooksExecution = new TaskExecutionMonitor(DoLoadBooksAsync);
        }

        public virtual void OnInitialize()
        {
            loadBooksExecution.Start();
        }

        protected virtual void DoStartPlay(BookItemViewModel book)
        {
            var context = new StartPlayInteractionRequestContext(book.Id);

            StartPlayRequest.Raise(context, () =>
            {
                ;
            });
        }

        protected abstract bool FilterSourceBook(BookItem bookItem);

        protected virtual void OnConnected()
        {
            //var token = BrowserServiceConnector.Library.Subscribe(BindSourceBooks);
        }

        protected virtual BookItemViewModel BuildBookItemModel(BookItem bookItem)
        {
            var model = new BookItemViewModel(bookItem.Id)
            {
                Title = bookItem.Title,
                //Authors = GetAuthorsForBook(book.Authors),
                //Synopsis = book.Synopsis,
                Duration = bookItem.Duration
            };

            if (0 < bookItem.Covers.Length)
            {
                model.ImageSource = (cancellationToken) => CoverService.GetImageAsync(bookItem.Covers[0], cancellationToken);
            }

            return model;
        }

        protected virtual void BindSourceBooks(IReadOnlyList<BookItem> bookItems)
        {
            Books.Clear();

            for (var index = 0; index < bookItems.Count; index++)
            {
                var bookItem = bookItems[index];

                if (false == FilterSourceBook(bookItem))
                {
                    continue;
                }

                var model = BuildBookItemModel(bookItem);

                Books.Add(model);
            }
        }

        private async Task DoLoadBooksAsync()
        {
            await BrowserServiceConnector.ConnectAsync();

            var library = await BrowserServiceConnector.GetLibraryAsync();

            BindSourceBooks(library);
        }

        private void OnMediaBrowserConnected(Unit _) => OnConnected();
    }
}