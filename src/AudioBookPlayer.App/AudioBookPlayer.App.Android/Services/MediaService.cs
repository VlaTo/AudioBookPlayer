using Android.App;
using Android.Content.PM;
using Android.Graphics;
using Android.Provider;
using Android.Support.V4.Content;
using AudioBookPlayer.App.Services;
using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Xamarin.Essentials;
using static Xamarin.Essentials.Permissions;

namespace AudioBookPlayer.App.Droid.Services
{
    internal sealed class MediaService : IMediaService
    {
        public async Task<string> GetRootFolderAsync()
        {
            // content://com.android.providers.downloads.documents/document/raw%3A%2Fstorage%2Femulated%2F0%2FDownload%2Fbook.m4b
            // raw:/storage/emulated/0/Download/book.m4b

            // /storage/emulated/0/Download
            var path = Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryDownloads).AbsolutePath;
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

            var folder1 = Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryDownloads);
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

            //var search = Android.Net.Uri.FromFile(folder1);
            var search = uri5;
            var cursor = Application.Context.ApplicationContext.ContentResolver.Query(search, columns, null, null, null);

            if (0 < cursor.Count)
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

            //var uri = MediaStore.Files.GetContentUri("external");
            //var uri1 = MediaStore.Audio.Media.GetContentUriForPath("extenal");
            //var uri2 = MediaStore.Audio.Media.GetContentUri("extenal");
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

            var temp1 = Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryDocuments).AbsolutePath;
            var temp2 = Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryDownloads).AbsolutePath;
            var temp3 = Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryMusic).AbsolutePath;
            //var uri = MediaStore.Audio.Media.GetContentUriForPath(temp3);
            //var uri = MediaStore.Audio.Media.GetContentUri(temp3);
            var uri1 = MediaStore.Audio.Media.ExternalContentUri;
            var uri2 = MediaStore.Audio.Media.InternalContentUri;

            /*var permission = ContextCompat.CheckSelfPermission(Application.Context, Android.Manifest.Permission.ReadExternalStorage);

            if ((int)Permission.Granted == permission)
            {
            }*/

            var files = Directory.EnumerateFiles(temp2);

            foreach(var file in files)
            {
                var extension = System.IO.Path.GetExtension(file);
                
                System.Diagnostics.Debug.WriteLine($"[MediaService] [LoadMediaAsync] File: '{file}'");

                if (String.Equals("m4b", extension))
                {

                }
            }

            // content://media/internal/audio/media

            // content://media/external/file
            // content://com.android.externalstorage.documents/document/primary%3AMusic
            // content://com.android.providers.media.documents/document/audio_root

            var resolver = Application.Context.ApplicationContext.ContentResolver;
            var cursor = resolver.Query(
                uri1,
                columns,
                //$"{MediaStore.Files.FileColumns.MediaType} = {(int)MediaType.Audio}",
                null,
                null,
                $"{MediaStore.Files.FileColumns.DateAdded} DESC"
            );

            if (0 < cursor.Count)
            {
                while (cursor.MoveToNext())
                {
                    try
                    {
                        var id = cursor.GetInt(cursor.GetColumnIndex(MediaStore.Files.FileColumns.Id));
                        //var mediaType = cursor.GetInt(cursor.GetColumnIndex(MediaStore.Files.FileColumns.MediaType));
                        var title = cursor.GetString(cursor.GetColumnIndex(MediaStore.Files.FileColumns.Title));
                        var displayName = cursor.GetString(cursor.GetColumnIndex(MediaStore.Files.FileColumns.DisplayName));

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

                    }
                }
            }
        }

    }
}