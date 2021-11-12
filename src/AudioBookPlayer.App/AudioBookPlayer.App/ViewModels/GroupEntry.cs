using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace AudioBookPlayer.App.ViewModels
{
    public class GroupEntry<TModel> : ViewModelBase, IEnumerable<TModel>, INotifyCollectionChanged
    {
        private string title;

        public string Title
        {
            get => title;
            set => SetProperty(ref title, value);
        }

        public ObservableCollection<TModel> Entries
        {
            get;
        }

        public event NotifyCollectionChangedEventHandler CollectionChanged
        {
            add => Entries.CollectionChanged += value;
            remove => Entries.CollectionChanged -= value;
        }

        public GroupEntry()
        {
            Entries = new ObservableCollection<TModel>();
        }

        public IEnumerator<TModel> GetEnumerator() => Entries.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}