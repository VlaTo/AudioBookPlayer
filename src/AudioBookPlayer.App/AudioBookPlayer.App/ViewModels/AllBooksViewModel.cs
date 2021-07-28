using System;
using System.Collections.Generic;
using System.Reactive;
using AudioBookPlayer.App.Domain.Models;
using AudioBookPlayer.App.Domain.Services;
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
            IMediaLibrary mediaLibrary,
            IAudioBooksConsumer booksConsumer)
            : base(mediaLibrary)
        {
            this.booksConsumer = booksConsumer;

            subscription = booksConsumer.Subscribe(Observer.Create<IEnumerable<AudioBook>>(DoBindAudioBooks));
        }

        private void DoBindAudioBooks(IEnumerable<AudioBook> audioBooks)
        {
            Books.Clear();

            foreach (var audioBook in audioBooks)
            {
                var model = CreateAudioBookModel(audioBook);
                Books.Add(model);
            }
        }
    }
}
