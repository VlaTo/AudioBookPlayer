using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AudioBookPlayer.App.Services
{
    public interface IFileSystemItem
    {
        string Name
        {
            get;
        }

        string Path
        {
            get;
        }

        DateTime LastModified
        {
            get;
        }

        long Size
        {
            get;
        }

        bool IsFile
        {
            get;
        }

        bool IsDirectory
        {
            get;
        }

        Task<IReadOnlyCollection<IFileSystemItem>> EnumerateItemsAsync();
    }

    public interface IFileSystemSource : IFileSystemItem
    {
    }

    public interface IStorageSourceService
    {
        Task<IReadOnlyCollection<IFileSystemSource>> GetSourcesAsync();
    }
}
