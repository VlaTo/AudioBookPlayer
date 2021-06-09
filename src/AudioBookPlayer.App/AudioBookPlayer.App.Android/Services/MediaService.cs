using System;
using System.Threading.Tasks;
using Android.App;
using Android.Media;
using Android.OS;
using Android.Provider;
using AudioBookPlayer.App.Services;
using Environment = Android.OS.Environment;
using Uri = Android.Net.Uri;

namespace AudioBookPlayer.App.Android.Services
{
    internal sealed class MediaService : IMediaService
    {
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

        public void LoadMedia()
        {
            var collection = (Build.VERSION.SdkInt >= BuildVersionCodes.Q)
                ? MediaStore.Audio.Media.GetContentUri(MediaStore.VolumeExternal)
                : MediaStore.Audio.Media.ExternalContentUri;

            var projection = new[]
            {
                MediaStore.Audio.IAudioColumns.Track,
                MediaStore.Audio.Media.ContentType
            };

            //var uri = MediaStore.Files.GetContentUri("external");
            //var uri1 = MediaStore.Audio.Media.GetContentUriForPath("extenal");
            //var uri2 = MediaStore.Audio.Media.GetContentUri("extenal");
            /*var columns = new[]
            {
                MediaStore.Files.FileColumns.Id,
                MediaStore.Files.FileColumns.Title,
                MediaStore.Files.FileColumns.DisplayName,
                //MediaStore.Files.FileColumns.MediaType,
                MediaStore.Files.FileColumns.MimeType,
                MediaStore.Files.FileColumns.Size,
                MediaStore.Files.FileColumns.DateAdded
            };*/

            var sortOrder = MediaStore.Audio.IAudioColumns.Track + " ASC";

            // var temp1 = Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryDocuments).AbsolutePath;
            // var temp2 = Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryDownloads).AbsolutePath;
            // var temp3 = Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryMusic).AbsolutePath;
            //var uri = MediaStore.Audio.Media.GetContentUriForPath(temp3);
            //var uri = MediaStore.Audio.Media.GetContentUri(temp3);
            // var uri1 = MediaStore.Audio.Media.ExternalContentUri;
            // var uri2 = MediaStore.Audio.Media.InternalContentUri;

            /*var permission = ContextCompat.CheckSelfPermission(Application.Context, Android.Manifest.Permission.ReadExternalStorage);

            if ((int)Permission.Granted == permission)
            {
            }*/

            // content://media/internal/audio/media

            // content://media/external/file
            // content://com.android.externalstorage.documents/document/primary%3AMusic
            // content://com.android.providers.media.documents/document/audio_root

            var cursor = Application.Context.ApplicationContext.ContentResolver.Query(
                collection,
                projection,
                null,
                null,
                sortOrder
            );

            if (0 < cursor.Count)
            {
                while (cursor.MoveToNext())
                {
                    try
                    {
                        var id = cursor.GetInt(cursor.GetColumnIndex(MediaStore.Audio.IAudioColumns.Track));
                        var displayName = cursor.GetString(cursor.GetColumnIndex(MediaStore.Audio.Media.ContentType));

                        //Bitmap bitmap = null;

                        /*switch (mediaType)
                        {
                            case (int)MediaType.Image:
                            {
                                bitmap = MediaStore.Images.Thumbnails.GetThumbnail(resolver, id, ThumbnailKind.MiniKind, new BitmapFactory.Options
                                {
                                    InSampleSize = 4,
                                    InPurgeable = true
                                });

                                break;
                            }

                            case (int)MediaType.Video:
                            {
                                bitmap = MediaStore.Video.Thumbnails.GetThumbnail(resolver, id, VideoThumbnailKind.MiniKind, new BitmapFactory.Options
                                {
                                    InSampleSize = 4,
                                    InPurgeable = true
                                });

                                break;
                            }
                        }*/

                        System.Diagnostics.Debug.WriteLine($"[MediaService] [LoadMediaAsync] Audio: '{displayName}'");

                    }
                    catch (Exception exception)
                    {
                        ;
                    }
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