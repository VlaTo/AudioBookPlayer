using AudioBookPlayer.App.Models;
using AudioBookPlayer.App.Services;
using LibraProgramming.Xamarin.Dependency.Container.Attributes;
using System;
using System.Collections.Generic;
using System.Reactive;

namespace AudioBookPlayer.App.ViewModels
{
    internal sealed class RecentBooksViewModel : BooksViewModelBase
    {
        private readonly IAudioBooksConsumer booksConsumer;
        private IDisposable subscription;

        [PrefferedConstructor]
        public RecentBooksViewModel(
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
                /*var latest = await MediaLibrary.GetLatestBookActivityAsync(audioBook.Id.Value);
                var temp = DateTime.Now - latest.when;
                if (temp.TotalDays > 7)
                {
                    continue;
                }*/

                var model = CreateAudioBookModel(audioBook);

                Books.Add(model);
            }
        }
    }
}
