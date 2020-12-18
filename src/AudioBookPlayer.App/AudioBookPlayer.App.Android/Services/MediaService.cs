using Android.App;
using Android.Content.PM;
using Android.Graphics;
using Android.Provider;
using Android.Support.V4.Content;
using AudioBookPlayer.App.Services;
using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Xamarin.Essentials;
using static Xamarin.Essentials.Permissions;

namespace AudioBookPlayer.App.Droid.Services
{
    internal sealed class MediaService : IMediaService
    {
        public async Task LoadMediaAsync()
        {

            var uri = MediaStore.Files.GetContentUri("external");
            var columns = new[]
            {
                MediaStore.Files.FileColumns.Id,
                MediaStore.Files.FileColumns.Title,
                MediaStore.Files.FileColumns.DisplayName,
                MediaStore.Files.FileColumns.MediaType,
                MediaStore.Files.FileColumns.MimeType,
                MediaStore.Files.FileColumns.Size,
                MediaStore.Files.FileColumns.DateAdded
            };

            /*var permission = ContextCompat.CheckSelfPermission(Application.Context, Android.Manifest.Permission.ReadExternalStorage);

            if ((int)Permission.Granted == permission)
            {
            }*/

            var resolver = Application.Context.ApplicationContext.ContentResolver;
            var cursor = resolver.Query(
                uri,
                columns,
                $"{MediaStore.Files.FileColumns.MediaType} = {(int)MediaType.Audio}",
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
                        var mediaType = cursor.GetInt(cursor.GetColumnIndex(MediaStore.Files.FileColumns.MediaType));

                        Bitmap bitmap = null;

                        switch (mediaType)
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
                        }
                    }
                    catch (Exception exception)
                    {

                    }
                }
            }
        }

    }
}