using System;
using System.Collections.Generic;
using AudioBookPlayer.App.Models;

namespace AudioBookPlayer.App.Services
{
    public interface IAudioBooksConsumer : IObservable<IEnumerable<AudioBook>>
    {
    }

    public interface IAudioBooksPublisher : IObserver<IEnumerable<AudioBook>>
    {
    }
}