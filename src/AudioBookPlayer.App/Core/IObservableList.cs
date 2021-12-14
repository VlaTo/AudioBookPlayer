using System;

namespace AudioBookPlayer.App.Core
{
    internal interface IObservableList<T>
    {
        IDisposable Subscribe(IListObserver<T> observer);
    }
}