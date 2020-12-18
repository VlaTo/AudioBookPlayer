using AudioBookPlayer.App.Services;
using LibraProgramming.Xamarin.Dependency.Container.Attributes;
using LibraProgramming.Xamarin.Interaction;
using LibraProgramming.Xamarin.Interaction.Contracts;
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

    public sealed class CloseInteractionRequestContext : InteractionRequestContext<string>
    {
        public CloseInteractionRequestContext(string path)
            : base(path)
        {
        }
    }

    public sealed class FolderPickerViewModel : ViewModelBase, IInitialize
    {
        private const string driveRoot = "/storage/emulated";

        private readonly IPermissionRequestor requestor;
        private readonly IMediaService mediaService;
        private readonly InteractionRequest<CloseInteractionRequestContext> closeRequest;
        private string currentPath;

        public ObservableCollection<FileSystemItem> Items
        {
            get;
        }

        public IInteractionRequest CloseRequest => closeRequest;

        public ICommand SelectItem
        {
            get;
        }

        public ICommand Apply
        {
            get;
        }

        [PrefferedConstructor]
        public FolderPickerViewModel(
            IPermissionRequestor requestor, 
            IMediaService mediaService)
        {
            this.requestor = requestor;
            this.mediaService = mediaService;

            closeRequest = new InteractionRequest<CloseInteractionRequestContext>();
            Items = new ObservableCollection<FileSystemItem>();
            SelectItem = new Command<FileSystemItem>(DoSelectItem);
            Apply = new Command(DoApply);
        }

        void IInitialize.OnInitialize()
        {
            currentPath = "/";

            foreach(var drive in Directory.GetLogicalDrives())
            {
                System.Diagnostics.Debug.WriteLine($"[ChooseLibraryFolderViewModel] [OnInitialize] Drive: '{drive}'");
            }

            var path = String.Empty;

            path = System.Environment.GetFolderPath(Environment.SpecialFolder.MyMusic);
            System.Diagnostics.Debug.WriteLine($"[ChooseLibraryFolderViewModel] [OnInitialize] My Music: '{path}'");
            
            path = System.Environment.GetFolderPath(Environment.SpecialFolder.CommonMusic);
            System.Diagnostics.Debug.WriteLine($"[ChooseLibraryFolderViewModel] [OnInitialize] Common Music: '{path}'");

            // content://com.android.externalstorage.documents/document/primary%3AMusic
            // content://com.android.providers.media.documents/document/audio_root
            // var backingFile = Path.Combine(Xamarin.Essentials.FileSystem.AppDataDirectory, "count.txt");

            Task.Run(async () =>
            {
                await mediaService.LoadMediaAsync();

                //var items = EnumerateFileSystemItems();
                var items = Array.Empty<FileSystemItem>();

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

                    //var items = EnumerateFileSystemItems();
                    var items = Array.Empty<FileSystemItem>();

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

        private void DoApply(object obj)
        {
            var context = new CloseInteractionRequestContext(String.Empty);

            closeRequest.Raise(context);

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
