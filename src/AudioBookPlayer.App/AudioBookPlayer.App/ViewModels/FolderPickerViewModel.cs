using AudioBookPlayer.App.Services;
using LibraProgramming.Xamarin.Core;
using LibraProgramming.Xamarin.Dependency.Container.Attributes;
using LibraProgramming.Xamarin.Interaction;
using LibraProgramming.Xamarin.Interaction.Contracts;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace AudioBookPlayer.App.ViewModels
{
    /// <summary>
    /// 
    /// </summary>
    public class FileSystemItemViewModel : ViewModelBase
    {
        private string title;
        private long length;
        private DateTime lastModified;

        public string Title
        {
            get => title;
            set => SetProperty(ref title, value);
        }

        public DateTime LastModified
        {
            get => lastModified;
            set => SetProperty(ref lastModified, value);
        }

        public long Length
        {
            get => length;
            set => SetProperty(ref length, value);
        }

        public bool IsFile => FileSystemItem.IsFile;

        public bool IsDirectory => FileSystemItem.IsDirectory;

        protected IFileSystemItem FileSystemItem
        {
            get;
        }

        protected FileSystemItemViewModel(IFileSystemItem item)
        {
            FileSystemItem = item;
            LastModified = item.LastModified;
            Length = -1;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public sealed class FolderItemViewModel : FileSystemItemViewModel
    {
        private int childCount;

        public int ChildCount
        {
            get => childCount;
            set => SetProperty(ref childCount, value);
        }

        public string Path
        {
            get => FileSystemItem.Path;
        }

        public FolderItemViewModel(IFileSystemItem folder)
            : base(folder)
        {
            Title = folder.Name;
        }

        public async Task<IReadOnlyCollection<FileSystemItemViewModel>> EnumerateItemsAsync()
        {
            var items = new List<FileSystemItemViewModel>();

            foreach (var item in await FileSystemItem.EnumerateItemsAsync()) {
                if (item.IsFile)
                {
                    var file = new FileItemViewModel(item);
                    
                    items.Add(file);

                    continue;
                }

                if (item.IsDirectory)
                {
                    var folder = new FolderItemViewModel(item);

                    items.Add(folder);

                    continue;
                }
            }

            return items.AsReadOnly();
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public sealed class FileItemViewModel : FileSystemItemViewModel
    {
        public FileItemViewModel(IFileSystemItem file)
            : base(file)
        {
            Title = file.Name;
            Length = file.Size;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public sealed class CloseInteractionRequestContext : InteractionRequestContext<string>
    {
        public CloseInteractionRequestContext(string path)
            : base(path)
        {
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public sealed class FolderPickerViewModel : ViewModelBase, IInitialize
    {
        private readonly IPermissionRequestor requestor;
        private readonly IStorageSourceService storageService;
        private readonly StringComparer comparer;

        public ObservableCollection<FileSystemItemViewModel> Items
        {
            get;
        }

        public ObservableStack<FileSystemItemViewModel> ReturnStack
        {
            get;
        }

        public InteractionRequest<CloseInteractionRequestContext> CloseRequest
        {
            get;
        }

        public Command LevelUp
        {
            get;
        }

        public Command<FileSystemItemViewModel> SelectItem
        {
            get;
        }

        public Command Apply
        {
            get;
        }

        [PrefferedConstructor]
        public FolderPickerViewModel(
            IPermissionRequestor requestor, 
            IStorageSourceService storageService)
        {
            comparer = StringComparer.Create(CultureInfo.CurrentUICulture, false);

            this.requestor = requestor;
            this.storageService = storageService;

            CloseRequest = new InteractionRequest<CloseInteractionRequestContext>();
            Items = new ObservableCollection<FileSystemItemViewModel>();
            ReturnStack = new ObservableStack<FileSystemItemViewModel>();
            LevelUp = new Command(DoLevelUp);
            SelectItem = new Command<FileSystemItemViewModel>(DoSelectItem);
            Apply = new Command(DoApply);
        }

        /*public async Task InitializeAsync()
        {
            await UpdateItemsAsync();
        }*/

        /*
        foreach(var drive in Directory.GetLogicalDrives())
        {
            System.Diagnostics.Debug.WriteLine($"[ChooseLibraryFolderViewModel] [OnInitialize] Drive: '{drive}'");
        }

        var path = String.Empty;

        path = System.Environment.GetFolderPath(Environment.SpecialFolder.MyMusic);
        System.Diagnostics.Debug.WriteLine($"[ChooseLibraryFolderViewModel] [OnInitialize] My Music: '{path}'");

        path = System.Environment.GetFolderPath(Environment.SpecialFolder.CommonMusic);
        System.Diagnostics.Debug.WriteLine($"[ChooseLibraryFolderViewModel] [OnInitialize] Common Music: '{path}'");
        */

        // content://com.android.externalstorage.documents/document/primary%3AMusic
        // content://com.android.providers.media.documents/document/audio_root
        // var backingFile = Path.Combine(Xamarin.Essentials.FileSystem.AppDataDirectory, "count.txt");
        // content://com.android.providers.downloads.documents/document/raw:/storage/emulated/0/Download/book.m4b
        // content://com.android.providers.downloads.documents/document/raw%3A%2Fstorage%2Femulated%2F0%2FDownload%2Fbook.m4b
        // raw:/storage/emulated/0/Download/book.m4b

        async void IInitialize.OnInitialize()
        {
            await UpdateItemsAsync();
        }

        private async void DoLevelUp(object _)
        {
            if (0 == ReturnStack.Count)
            {
                return;
            }

            ReturnStack.Pop();

            await UpdateItemsAsync();
        }

        private async void DoSelectItem(FileSystemItemViewModel item)
        {
            switch (item)
            {
                case FolderItemViewModel folder:
                {
                    ReturnStack.Push(folder);

                    await UpdateItemsAsync();

                    break;
                }

                case FileItemViewModel file:
                {
                    System.Diagnostics.Debug.WriteLine($"[ChooseLibraryFolderViewModel] [DoSelectItem] File: '{file.Title}'");

                    break;
                }
            }
        }

        private void DoApply(object _)
        {
            var current = ReturnStack.Peek();

            if (current is FolderItemViewModel folder)
            {
                var context = new CloseInteractionRequestContext(folder.Path);

                CloseRequest.Raise(context);
            }
        }

        private async Task UpdateItemsAsync()
        {
            Items.Clear();

            if (0 == ReturnStack.Count)
            {
                var sources = await storageService.GetSourcesAsync();

                BindViewModels(sources.Select(source => CreateModelFrom(source)));
                
                return;
            }

            var current = ReturnStack.Peek();

            if (current is FolderItemViewModel folder)
            {
                var models = await folder.EnumerateItemsAsync();

                BindViewModels(models);
            }
        }

        private FileSystemItemViewModel CreateModelFrom(IFileSystemItem item)
        {
            if (null == item)
            {
                throw new ArgumentNullException(nameof(item));
            }

            if (item.IsFile)
            {
                return new FileItemViewModel(item);
            }

            if (item.IsDirectory)
            {
                return new FolderItemViewModel(item);
            }

            throw new Exception();
        }

        private void BindViewModels(IEnumerable<FileSystemItemViewModel> models)
        {
            int FindIndex(FileSystemItemViewModel model)
            {
                if (0 == Items.Count)
                {
                    return 0;
                }

                if (model.IsDirectory)
                {
                    for (var index = 0; index < Items.Count; index++)
                    {
                        if (Items[index].IsFile)
                        {
                            return index;
                        }

                        if (Items[index].IsDirectory)
                        {
                            var result = comparer.Compare(Items[index].Title, model.Title);

                            if (0 <= result)
                            {
                                continue;
                            }
                        }
                    }
                }

                if (model.IsFile)
                {
                    for (var index = 0; index < Items.Count; index++)
                    {
                        if (Items[index].IsFile)
                        {
                            var result = comparer.Compare(Items[index].Title, model.Title);

                            if (0 > result)
                            {
                                return index;
                            }
                        }

                        if (Items[index].IsDirectory)
                        {
                            continue;
                        }
                    }
                }

                return Items.Count;
            }

            foreach (var model in models)
            {
                var index = FindIndex(model);
                Items.Insert(index, model);
            }
        }
    }
}
