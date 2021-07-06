using System;
using System.Collections.Generic;
using System.Reactive;
using AudioBookPlayer.App.Models;
using AudioBookPlayer.App.Services;
using LibraProgramming.Xamarin.Dependency.Container.Attributes;

namespace AudioBookPlayer.App.ViewModels
{
    internal sealed class AllBooksViewModel : BooksViewModelBase
    {
        private readonly IAudioBooksConsumer booksConsumer;
        private IDisposable subscription;

        [PrefferedConstructor]
        public AllBooksViewModel(
            IBookShelfProvider bookShelf,
            IAudioBooksConsumer booksConsumer)
            : base(bookShelf)
        {
            this.booksConsumer = booksConsumer;

            subscription = booksConsumer.Subscribe(Observer.Create<IEnumerable<AudioBook>>(DoNextAudioBook));
        }

        protected override void DoPlayBook(AudioBookViewModel book)
        {
            base.DoPlayBook(book);
        }

        private void DoNextAudioBook(IEnumerable<AudioBook> audioBooks)
        {
            Books.Clear();

            foreach (var audioBook in audioBooks)
            {
                AddBookToList(audioBook);
            }
        }
    }
}
