using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace AudioBookPlayer.App.ViewModels
{
    public class GroupEntry<TModel> : ViewModelBase, IEnumerable<TModel>
    {
        private string title;

        public string Title
        {
            get => title;
            set => SetProperty(ref title, value);
        }

        public ICollection<TModel> Entries
        {
            get;
        }

        public GroupEntry()
        {
            Entries = new Collection<TModel>();
        }

        public IEnumerator<TModel> GetEnumerator() => Entries.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}