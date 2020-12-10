using LibraProgramming.Xamarin.Dependency.Container.Attributes;
using LibraProgramming.Xamarin.Interaction;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Xamarin.Forms;

namespace AudioBookPlayer.App.ViewModels
{
    public sealed class ChooseLibraryFolderViewModel : ViewModelBase, IInitialize
    {
        public ObservableCollection<string> Items
        {
            get;
        }

        public ICommand SelectItem
        {
            get;
        }

        [PrefferedConstructor]
        public ChooseLibraryFolderViewModel()
        {
            Items = new ObservableCollection<string>();
            SelectItem = new Command<string>(DoSelectItem);
        }

        void IInitialize.OnInitialize()
        {
            Items.Add("Lorem Ipsum");
            Items.Add("Dolor sit amet");
            Items.Add("jvfkjgvhfjc sdkfcj");
        }

        private void DoSelectItem(string args)
        {
            System.Diagnostics.Debug.WriteLine($"[ChooseLibraryFolderViewModel] [DoSelectItem] Item: '{args}'");
        }
    }
}
