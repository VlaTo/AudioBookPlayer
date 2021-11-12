using AudioBookPlayer.App.Core.Extensions;
using AudioBookPlayer.App.Domain.Models;
using AudioBookPlayer.App.Domain.Services;
using AudioBookPlayer.App.Services;
using AudioBookPlayer.App.ViewModels.RequestContexts;
using LibraProgramming.Xamarin.Interaction;
using LibraProgramming.Xamarin.Interaction.Contracts;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Xamarin.Forms;

namespace AudioBookPlayer.App.ViewModels
{
    /// <summary>
    /// 
    /// </summary>
    internal abstract class BooksViewModelBase : ViewModelBase, IInitialize, IBooksViewModel, ILibraryCallback, ICleanup
    {
        private IDisposable librarySubscription;
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

        protected IMediaBrowserServiceConnector ServiceConnector
        {
            get;
        }

        protected ICoverService CoverService
        {
            get;
        }

        protected BooksViewModelBase(
            IMediaBrowserServiceConnector serviceConnector,
            ICoverService coverService)
        {
            ServiceConnector = serviceConnector;
            CoverService = coverService;
            Books = new ObservableCollection<BookItemViewModel>();
            StartPlay = new Command<BookItemViewModel>(DoStartPlay);
            StartPlayRequest = new InteractionRequest<StartPlayInteractionRequestContext>();
        }

        public virtual void OnInitialize()
        {
            librarySubscription = ServiceConnector.Subscribe(this);
        }

        public virtual void OnCleanup()
        {
            if (null != librarySubscription)
            {
                librarySubscription.Dispose();
                librarySubscription = null;
            }
        }

        void ILibraryCallback.OnGetBooks(IReadOnlyList<BookItem> books) => BindBooks(books);

        protected virtual void DoStartPlay(BookItemViewModel book)
        {
            var mediaId = new MediaId(book.Id);
            var context = new StartPlayInteractionRequestContext(mediaId);

            StartPlayRequest.Raise(context, () =>
            {
                ;
            });
        }

        protected abstract bool FilterSourceBook(BookItem bookItem);

        protected virtual BookItemViewModel BuildBookItemModel(BookItem bookItem)
        {
            var model = new BookItemViewModel(bookItem.Id)
            {
                Title = bookItem.Title,
                Authors = bookItem.Authors.ToCommaString(),
                Duration = bookItem.Duration
            };

            if (0 < bookItem.Covers.Length)
            {
                model.ImageSource = (cancellationToken) => CoverService.GetImageAsync(bookItem.Covers[0], cancellationToken);
            }

            return model;
        }

        protected virtual void BindBooks(IReadOnlyList<BookItem> bookItems)
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
    }
}