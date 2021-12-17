using System.Collections.Generic;

namespace AudioBookPlayer.App.Core
{
    public interface IListObserver<in T>
    {
        void OnAdded(int position, T value);
        
        void OnRangeAdded(int position, IEnumerable<T> values);
        
        void OnReplace(int position, T oldValue, T newValue);

        void OnRemove(int position, T value);

        void OnClear();
    }
}