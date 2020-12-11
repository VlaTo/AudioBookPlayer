using AudioBookPlayer.App.Services;
using LibraProgramming.Xamarin.Dependency.Container.Attributes;
using LibraProgramming.Xamarin.Interaction;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace AudioBookPlayer.App.ViewModels
{
    public abstract class FileSystemItem : ViewModelBase
    {
        private string title;
        private DateTime created;

        public string Title
        {
            get => title;
            set => SetProperty(ref title, value);
        }

        public DateTime Created
        {
            get => created;
            set => SetProperty(ref created, value);
        }
    }

    public sealed class FolderItem : FileSystemItem
    {
        private int childCount;

        public int ChildCount
        {
            get => childCount;
            set => SetProperty(ref childCount, value);
        }
    }

    public sealed class FileItem : FileSystemItem
    {
        private long length;

        public long Length
        {
            get => length;
            set => SetProperty(ref length, value);
        }
    }

    public sealed class ChooseLibraryFolderViewModel : ViewModelBase, IInitialize
    {
        private const string driveRoot = "/storage/emulated";

        private readonly IPermissionRequestor requestor;
        private string currentPath;

        public ObservableCollection<FileSystemItem> Items
        {
            get;
        }

        public ICommand SelectItem
        {
            get;
        }

        [PrefferedConstructor]
        public ChooseLibraryFolderViewModel(IPermissionRequestor requestor)
        {
            this.requestor = requestor;

            Items = new ObservableCollection<FileSystemItem>();
            SelectItem = new Command<FileSystemItem>(DoSelectItem);
        }

        void IInitialize.OnInitialize()
        {
            currentPath = "/";

            foreach(var drive in Directory.GetLogicalDrives())
            {
                System.Diagnostics.Debug.WriteLine($"[ChooseLibraryFolderViewModel] [OnInitialize] Drive: '{drive}'");
            }

            Task.Run(async () =>
            {
                await requestor.CheckAndRequestMediaPermissionsAsync();

                var items = EnumerateFileSystemItems();

                Items.Clear();

                foreach (var item in items)
                {
                    Items.Add(item);
                }
            });
        }

        private void DoSelectItem(FileSystemItem item)
        {
            switch (item)
            {
                case FolderItem folder:
                {
                    currentPath = Path.Combine(currentPath, folder.Title);

                    var items = EnumerateFileSystemItems();

                    Items.Clear();

                    foreach (var fi in items)
                    {
                        Items.Add(fi);
                    }

                    break;
                }

                case FileItem file:
                {
                    System.Diagnostics.Debug.WriteLine($"[ChooseLibraryFolderViewModel] [DoSelectItem] File: '{file.Title}'");

                    break;
                }
            }
        }

        private IReadOnlyCollection<FileSystemItem> EnumerateFileSystemItems()
        {
            var path = driveRoot + currentPath;
            var items = new List<FileSystemItem>();

            foreach (var folder in Directory.EnumerateDirectories(path))
            {
                var created = Directory.GetCreationTime(folder);

                items.Add(new FolderItem
                {
                    Title = folder,
                    Created = created
                });
            }

            foreach (var file in Directory.EnumerateFiles(path, "*.mp3,*.m4b"))
            {
                var length = 0L;

                if (false == File.Exists(file))
                {
                    continue;
                }

                using (var stream = File.OpenRead(path))
                {
                    length = stream.Length;
                }

                items.Add(new FileItem
                {
                    Title = file,
                    Length = length
                });
            }

            return items.AsReadOnly();
        }
    }
}
