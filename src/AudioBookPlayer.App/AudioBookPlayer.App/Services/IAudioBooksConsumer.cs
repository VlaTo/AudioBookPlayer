using AudioBookPlayer.App.Domain.Models;
using System;
using System.Collections.Generic;

namespace AudioBookPlayer.App.Services
{
    public interface IAudioBooksConsumer : IObservable<IEnumerable<AudioBook>>
    {
    }

    public interface IAudioBooksPublisher : IObserver<IEnumerable<AudioBook>>
    {
    }
}