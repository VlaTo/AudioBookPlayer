using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Provider;
using AudioBookPlayer.App.Android.Core;
using AudioBookPlayer.App.Domain.Services;
using Uri = Android.Net.Uri;

namespace AudioBookPlayer.App.Android.Services
{
    internal sealed class CoverProvider : ICoverProvider
    {
        private const string FileMode = "w";

        private readonly Uri collectionUri;
        private readonly ContentResolver resolver;

        public CoverProvider()
        {
            collectionUri = GetExternalContentUri();
            resolver = Application.Context.ContentResolver;
        }

        public async Task<string> AddCoverAsync(long bookId, Stream stream)
        {
            var displayName = Guid.NewGuid().ToString("N");
            var mimeType = await GetMimeTypeAsync(stream);
            var values = new ContentValues();

            values.Put(MediaStore.IMediaColumns.DocumentId, bookId);
            values.Put(MediaStore.IMediaColumns.DisplayName, displayName);
            values.Put(MediaStore.IMediaColumns.MimeType, mimeType);
            values.Put(MediaStore.IMediaColumns.IsPending, true);

            var contentUri = resolver.Insert(collectionUri, values);

            using (var afd = resolver.OpenAssetFileDescriptor(contentUri, FileMode))
            {
                using (var output = afd.CreateOutputStream())
                {
                    await stream.CopyToAsync(output);
                    await output.FlushAsync();
                }
            }

            values.Clear();
            values.Put(MediaStore.IMediaColumns.IsPending, false);

            resolver.Update(contentUri, values, null);

            return contentUri.ToString();
        }

        private static Uri GetExternalContentUri()
            => (Build.VERSION.SdkInt >= BuildVersionCodes.Q)
                ? MediaStore.Images.Media.GetContentUri(MediaStore.VolumeExternal)
                : MediaStore.Images.Media.ExternalContentUri;

        private static async Task<string> GetMimeTypeAsync(Stream stream)
        {
            var dict = new[]
            {
                new KeyValuePair<byte[], string>(new byte[] {0xFF, 0xD8, 0xFF }, "image/jpeg"),
                new KeyValuePair<byte[], string>(new byte[] {0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A }, "image/png"),
                new KeyValuePair<byte[], string>(new byte[] {0x47, 0x49, 0x46 }, "image/gif")
            };
            
            var buffer = new byte[8];
            var count = await stream.ReadAsync(buffer, 0, buffer.Length);
            var mimeType = "image/*";

            for (var index = 0; index < dict.Length; index++)
            {
                var kvp = dict[index];

                if (AreSame(buffer, kvp.Key))
                {
                    mimeType = kvp.Value;
                    break;
                }
            }

            stream.Seek(0L, SeekOrigin.Begin);

            return mimeType;
        }

        private static bool AreSame(byte[] source, byte[] expected)
        {
            for (int index = 0; index < expected.Length; index++)
            {
                if (source[index] == expected[index])
                {
                    continue;
                }

                return false;
            }

            return true;
        }
    }
}