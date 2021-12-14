namespace AudioBookPlayer.App.Core
{
    public interface IListObserver<in T>
    {
        void OnAdded(int position, T item);
        
        void OnReplace(int position, T item);

        void OnRemove(int position);

        void OnClear();
    }
}