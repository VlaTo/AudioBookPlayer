using System;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Media;
using Android.OS;
using Android.Provider;
using AndroidX.Core.Content;
using AudioBookPlayer.App.Services;
using Java.IO;
using Java.Util;
using Xamarin.Essentials;
using Environment = Android.OS.Environment;
using FileProvider = Xamarin.Essentials.FileProvider;
using Uri = Android.Net.Uri;

namespace AudioBookPlayer.App.Android.Services
{
    internal sealed class MediaService : IMediaService
    {
        private readonly IPermissionRequestor permissionRequestor;

        public MediaService(IPermissionRequestor permissionRequestor)
        {
            this.permissionRequestor = permissionRequestor;
        }

        public async Task<string> GetRootFolderAsync()
        {
            // content://com.android.providers.downloads.documents/document/raw%3A%2Fstorage%2Femulated%2F0%2FDownload%2Fbook.m4b
            // raw:/storage/emulated/0/Download/book.m4b

            // /storage/emulated/0/Download
            var path = Environment.GetExternalStoragePublicDirectory(Environment.DirectoryDownloads).AbsolutePath;
            // content://media//storage/emulated/0/Download/file
            var uri1 = MediaStore.Files.GetContentUri(path);
            // content://media/external/audio/media
            var uri3 = MediaStore.Audio.Media.GetContentUriForPath(path);
            // content://media/temp/file
            var uri2 = MediaStore.Files.GetContentUri("temp");
            // content://media/internal/audio/media
            var uri4 = MediaStore.Audio.Media.GetContentUriForPath("temp");
            // content://media/external/audio/media
            var uri5 = MediaStore.Audio.Media.ExternalContentUri;
            // content://media/internal/audio/media
            var uri6 = MediaStore.Audio.Media.InternalContentUri;

            var folder1 = Environment.GetExternalStoragePublicDirectory(Environment.DirectoryDownloads);
            var temp1 = folder1.CanonicalPath;
            var temp2 = folder1.ToPath();
            var temp3 = await folder1.ListFilesAsync();

            foreach (var item in temp3)
            {
                if (item.IsDirectory)
                {

                    continue;
                }

                if (item.IsFile)
                {

                    continue;
                }
            }

            var columns = new[]
            {
                MediaStore.Files.FileColumns.Id,
                MediaStore.Files.FileColumns.Title,
                MediaStore.Files.FileColumns.DisplayName,
                //MediaStore.Files.FileColumns.MediaType,
                MediaStore.Files.FileColumns.MimeType,
                MediaStore.Files.FileColumns.Size,
                MediaStore.Files.FileColumns.DateAdded
            };

            //var temp22 = Android.Net.Uri.FromFile(folder1);
            //var temp23 = MediaStore.GetDocumentUri(Application.Context, temp22);
            var search = Uri.Parse("raw:/storage/emulated/0/Download/");
            //var search = uri5;
            var cursor = Application.Context.ApplicationContext.ContentResolver.Query(search, columns, null, null, null);

            if (null != cursor && 0 < cursor.Count)
            {
                while (cursor.MoveToNext())
                {
                    var displayName = cursor.GetString(cursor.GetColumnIndex(MediaStore.Files.FileColumns.DisplayName));

                }
            }

            return path;
        }

        public async Task LoadMediaAsync()
        {
            var externalVolumes = MediaStore.GetExternalVolumeNames(Application.Context);

            foreach (var volume in externalVolumes)
            {
                // MediaStore.GetMediaUri()
                System.Diagnostics.Debug.WriteLine($"[MediaService] [LoadMediaAsync] Volume: '{volume}'");
            }

            var collection = /*(Build.VERSION.SdkInt >= BuildVersionCodes.Q)
                ? MediaStore.Audio.Media.GetContentUri(MediaStore.VolumeExternal)
                : MediaStore.Audio.Media.ExternalContentUri;*/
                MediaStore.Files.GetContentUri(MediaStore.VolumeExternalPrimary);

            if (null == collection)
            {
                return;
            }

            var columns = new[]
            {
                MediaStore.Audio.Media.InterfaceConsts.Id,
                MediaStore.IMediaColumns.VolumeName,
                MediaStore.Files.IFileColumns.MediaType,
                MediaStore.Files.IFileColumns.MimeType,
                MediaStore.Files.IFileColumns.Title,
                MediaStore.Files.IFileColumns.Parent,
                MediaStore.IMediaColumns.DateModified
            };

            var permission = await permissionRequestor.CheckAndRequestMediaPermissionsAsync();

            if (permission != PermissionStatus.Granted)
            {
                return;
            }

            var contentResolver = Application.Context.ContentResolver;
            // const string sortOrder = MediaStore.Audio.Media.InterfaceConsts.Id + " ASC";
            const string sortOrder = MediaStore.Audio.Media.InterfaceConsts.Id + " ASC";
            var cursor = contentResolver?.Query(collection, columns, null, null, sortOrder);

            for (var moved = null != cursor && cursor.MoveToFirst(); moved; moved = cursor.MoveToNext())
            {
                try
                {
                    var id = cursor.GetLong(cursor.GetColumnIndex(MediaStore.Audio.Media.InterfaceConsts.Id));
                    var volumeName = cursor.GetString(cursor.GetColumnIndex(MediaStore.IMediaColumns.VolumeName));
                    var mediaType = cursor.GetLong(cursor.GetColumnIndex(MediaStore.Files.IFileColumns.MediaType));
                    var mimeType = cursor.GetString(cursor.GetColumnIndex(MediaStore.Files.IFileColumns.MimeType));
                    var title = cursor.GetString(cursor.GetColumnIndex(MediaStore.Files.IFileColumns.Title));
                    var parent = cursor.GetLong(cursor.GetColumnIndex(MediaStore.Files.IFileColumns.Parent));
                    var dateModified = cursor.GetString(cursor.GetColumnIndex(MediaStore.IMediaColumns.DateModified));

                    System.Diagnostics.Debug.WriteLine($"[Audio] [{id}] '{volumeName}' {mediaType} '{mimeType}' '{title}' ({parent})");


                    /*var id = cursor.GetLong(cursor.GetColumnIndex(MediaStore.Audio.Media.InterfaceConsts.Id));
                    var volumeName = cursor.GetString(cursor.GetColumnIndex(MediaStore.IMediaColumns.VolumeName));
                    var artist = cursor.GetString(cursor.GetColumnIndex(MediaStore.Audio.Media.InterfaceConsts.Artist));
                    var album = cursor.GetString(cursor.GetColumnIndex(MediaStore.Audio.Media.InterfaceConsts.Album));
                    var title = cursor.GetString(cursor.GetColumnIndex(MediaStore.IMediaColumns.Title));
                    var relativePath = cursor.GetString(cursor.GetColumnIndex(MediaStore.IMediaColumns.RelativePath));

                    TimeSpan duration;
                    {
                        var value = cursor.GetLong(cursor.GetColumnIndex(MediaStore.IMediaColumns.Duration));
                        duration = TimeSpan.FromMilliseconds(value);
                    }

                    DateTime modified;
                    {
                        var value = cursor.GetLong(cursor.GetColumnIndex(MediaStore.IMediaColumns.DateModified));
                        modified = DateTime.UnixEpoch + TimeSpan.FromSeconds(value);
                    }

                    /--Uri documentUri;
                    {
                        var value = cursor.GetString(cursor.GetColumnIndex(MediaStore.IMediaColumns.DocumentId));
                        documentUri = DocumentsContract.BuildDocumentUri("com.android.providers.media.documents", value);
                    }--/

                    // get content
                    {
                        var contentUri = MediaStore.Audio.Media.GetContentUri(volumeName);
                        var uri = ContentUris.WithAppendedId(contentUri, id);
                        
                        using (var fd = contentResolver.OpenAssetFileDescriptor(uri, "r"))
                        {
                            System.Diagnostics.Debug.WriteLine($"[Audio] [{id}] '{album}' '{artist}' '{title}' {duration:g} ({uri}) [{relativePath}] {fd.Length:N0} {modified:g}");
                        }
                    }*/
                }
                catch (Exception exception)
                {
                    System.Diagnostics.Debug.WriteLine(exception.Message);
                }
            }
        }

        private void UpdateMediaStoreFileInfo(Action<string> callback, params string[] files)
        {
            MediaScannerConnection.ScanFile(
                Application.Context,
                files,
                null,
                new OnScanCompletedListener(callback)
            );
        }

        // MediaScannerConnection update listener
        private sealed class OnScanCompletedListener : Java.Lang.Object, MediaScannerConnection.IOnScanCompletedListener
        {
            private readonly Action<string> callback;

            public OnScanCompletedListener(Action<string> callback)
            {
                this.callback = callback;
            }

#nullable enable
            public void OnScanCompleted(string? path, Uri? uri)
            {
                callback.Invoke(path);
            }
#nullable restore
        }
    }
}