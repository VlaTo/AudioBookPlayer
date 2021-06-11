using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using AudioBookPlayer.App.Android.Core.Extensions;
using AudioBookPlayer.App.Services;
using Environment = Android.OS.Environment;

namespace AudioBookPlayer.App.Android.Services
{
    internal sealed class StorageSourceService : IStorageSourceService
    {
        private readonly IPermissionRequestor permissionRequestor;

        public StorageSourceService(IPermissionRequestor permissionRequestor)
        {
            this.permissionRequestor = permissionRequestor;
        }

        public async Task<IReadOnlyCollection<IFileSystemSource>> GetSourcesAsync()
        {
            var temp = new MediaService(permissionRequestor);

            await temp.LoadMediaAsync();

            var collection = new List<IFileSystemSource>
            {
                new SourceFolder(
                    this,
                    Environment.GetExternalStoragePublicDirectory(Environment.DirectoryDownloads)
                ),
                new SourceFolder(
                    this,
                    Environment.GetExternalStoragePublicDirectory(Environment.DirectoryMusic)
                )
            };

            return collection.AsReadOnly();
        }

        // 
        private sealed class SourceFolder : IFileSystemSource
        {
            private readonly StorageSourceService service;
            private readonly Java.IO.File folder;

            public string Name => folder.Name;

            public string Path => folder.AbsolutePath;

            public DateTime LastModified { get; }

            public bool IsFile { get; } = false;

            public bool IsDirectory { get; } = true;

            public long Size => folder.Length();

            public SourceFolder(StorageSourceService service, Java.IO.File folder)
            {
                this.service = service;
                this.folder = folder;
                //folder = Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryMusic);
                LastModified = folder.LastModified().ToFileTime();
            }

            public async Task<IReadOnlyCollection<IFileSystemItem>> EnumerateItemsAsync()
            {
                var files = new List<IFileSystemItem>();
                var sourceFiles = await folder.ListFilesAsync();

                var tenp = Directory.EnumerateFileSystemEntries(folder.AbsolutePath);

                if (null != sourceFiles)
                {
                    foreach (var item in sourceFiles)
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
                }

                return files;
            }
        }

        // DirectoryItem class
        private sealed class DirectoryItem : IFileSystemItem
        {
            private readonly SourceFolder owner;
            private readonly DirectoryItem parent;
            private readonly Java.IO.File folder;

            public string Name => folder.Name;

            public string Path => folder.AbsolutePath;

            public DateTime LastModified { get; }

            public bool IsFile => folder.IsFile;

            public bool IsDirectory => folder.IsDirectory;

            public long Size { get; } = -1;

            public DirectoryItem(SourceFolder owner, DirectoryItem parent, Java.IO.File folder)
            {
                this.owner = owner;
                this.parent = parent;
                this.folder = folder;

                LastModified = folder.LastModified().ToFileTime();
            }

            public async Task<IReadOnlyCollection<IFileSystemItem>> EnumerateItemsAsync()
            {
                var files = new List<IFileSystemItem>();
                var sourceFiles = await folder.ListFilesAsync();

                if (null != sourceFiles)
                {
                    foreach (var item in sourceFiles)
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
                }

                return files;
            }
        }

        // FileItem class
        private sealed class FileItem : IFileSystemItem
        {
            private readonly SourceFolder owner;
            private readonly DirectoryItem parent;
            private readonly Java.IO.File file;

            public string Name => file.Name;

            public string Path => file.AbsolutePath;

            public DateTime LastModified { get; }

            public bool IsFile => file.IsFile;

            public bool IsDirectory => file.IsDirectory;

            public long Size => file.Length();

            public FileItem(SourceFolder owner, DirectoryItem parent, Java.IO.File file)
            {
                this.owner = owner;
                this.parent = parent;
                this.file = file;

                LastModified = file.LastModified().ToFileTime();
            }

            public Task<IReadOnlyCollection<IFileSystemItem>> EnumerateItemsAsync()
            {
                return Task.FromResult<IReadOnlyCollection<IFileSystemItem>>(Array.Empty<IFileSystemItem>());
            }
        }
    }
}