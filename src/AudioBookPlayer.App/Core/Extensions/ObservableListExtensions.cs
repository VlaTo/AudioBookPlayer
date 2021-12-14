using System;
using System.Collections.Generic;
using System.Reactive.Disposables;

namespace AudioBookPlayer.App.Core.Extensions
{
    internal static class ObservableListExtensions
    {
        public static IDisposable Subscribe<T>(this IObservableList<T> source, Action<IReadOnlyList<T>> onNext, Action<Exception> onError)
        {

            return Disposable.Empty;
        }
    }
}