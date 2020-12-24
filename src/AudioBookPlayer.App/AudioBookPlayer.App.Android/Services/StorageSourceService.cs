using AudioBookPlayer.App.Droid.Core.Extensions;
using AudioBookPlayer.App.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AudioBookPlayer.App.Droid.Services
{
    internal sealed class StorageSourceService : IStorageSourceService
    {
        public StorageSourceService()
        {

        }

        public Task<IReadOnlyCollection<IFileSystemSource>> GetSourcesAsync()
        {
            var collection = new List<IFileSystemSource>();

            collection.Add(new DownloadStorageFolder(this));

            return Task.FromResult<IReadOnlyCollection<IFileSystemSource>>(collection.AsReadOnly());
        }

        private sealed class DownloadStorageFolder : IFileSystemSource
        {
            private readonly StorageSourceService service;
            private Java.IO.File folder;

            public string Name => folder.Name;

            public string Path => folder.AbsolutePath;

            public DateTime LastModified { get; }

            public bool IsFile { get; } = false;

            public bool IsDirectory { get; } = true;

            public long Size => folder.Length();

            public DownloadStorageFolder(StorageSourceService service)
            {
                this.service = service;

                folder = Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryDownloads);
                LastModified = FileDateTime.FromFileTime(folder.LastModified());
            }

        public async Task<IReadOnlyCollection<IFileSystemItem>> EnumerateItemsAsync()
            {
                var files = new List<IFileSystemItem>();

                foreach (var item in await folder.ListFilesAsync())
                {
                    if (item.IsHidden)
                    {
                        continue;
                    }

                    if (item.IsFile)
                    {
                        var directory = new FileItem(this, null, item);

                        files.Add(directory);

                        continue;
                    }

                    if (item.IsDirectory)
                    {
                        var directory = new DirectoryItem(this, null, item);

                        files.Add(directory);

                        continue;
                    }
                }

                return files;
            }
        }

        private sealed class DirectoryItem : IFileSystemItem
        {
            private readonly DownloadStorageFolder owner;
            private readonly DirectoryItem parent;
            private readonly Java.IO.File folder;

            public string Name => folder.Name;

            public string Path => folder.AbsolutePath;

            public DateTime LastModified { get; }

            public bool IsFile => folder.IsFile;

            public bool IsDirectory => folder.IsDirectory;

            public long Size { get; } = -1;

            public DirectoryItem(DownloadStorageFolder owner, DirectoryItem parent, Java.IO.File folder)
            {
                this.owner = owner;
                this.parent = parent;
                this.folder = folder;

                LastModified = FileDateTime.FromFileTime(folder.LastModified());
            }

            public async Task<IReadOnlyCollection<IFileSystemItem>> EnumerateItemsAsync()
            {
                var files = new List<IFileSystemItem>();

                foreach (var item in await folder.ListFilesAsync())
                {
                    if (item.IsHidden)
                    {
                        continue;
                    }

                    if (item.IsFile)
                    {
                        continue;
                    }

                    if (item.IsDirectory)
                    {
                        var directory = new DirectoryItem(owner, this, item);

                        files.Add(directory);

                        continue;
                    }
                }

                return files;
            }
        }

        private sealed class FileItem : IFileSystemItem
        {
            private readonly DownloadStorageFolder owner;
            private readonly DirectoryItem parent;
            private readonly Java.IO.File file;

            public string Name => file.Name;

            public string Path => file.AbsolutePath;

            public DateTime LastModified { get; }

            public bool IsFile => file.IsFile;

            public bool IsDirectory => file.IsDirectory;

            public long Size => file.Length();

            public FileItem(DownloadStorageFolder owner, DirectoryItem parent, Java.IO.File file)
            {
                this.owner = owner;
                this.parent = parent;
                this.file = file;

                LastModified = FileDateTime.FromFileTime(file.LastModified());
            }

            public Task<IReadOnlyCollection<IFileSystemItem>> EnumerateItemsAsync()
            {
                return Task.FromResult<IReadOnlyCollection<IFileSystemItem>>(Array.Empty<IFileSystemItem>());
            }
        }
    }
}