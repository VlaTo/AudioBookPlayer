using System;
using System.Collections.Generic;
using System.Reactive.Subjects;
using AudioBookPlayer.App.Models;

namespace AudioBookPlayer.App.Services
{
    internal sealed class AudioBooksHub : IAudioBooksPublisher, IAudioBooksConsumer
    {
        private readonly Subject<IEnumerable<AudioBook>> subject;

        public AudioBooksHub()
        {
            subject = new Subject<IEnumerable<AudioBook>>();
        }

        public void OnCompleted()
        {
            subject.OnCompleted();
        }

        public void OnError(Exception error)
        {
            subject.OnError(error);
        }

        public void OnNext(IEnumerable<AudioBook> value)
        {
            subject.OnNext(value);
        }

        public IDisposable Subscribe(IObserver<IEnumerable<AudioBook>> observer)
        {
            return subject.Subscribe(observer);
        }
    }
}