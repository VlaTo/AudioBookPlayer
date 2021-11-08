using Android.App;
using Android.Content;
using Android.OS;
using Android.Provider;
using AudioBookPlayer.App.Domain.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Uri = Android.Net.Uri;

namespace AudioBookPlayer.App.Android.Services
{
    internal sealed class CoverService : ICoverService
    {
        private const string WriteMode = "w";
        private const string ReadMode = "r";

        private readonly Uri collectionUri;
        private readonly ContentResolver resolver;

        public CoverService()
        {
            collectionUri = GetExternalContentUri();
            resolver = Application.Context.ContentResolver;
        }

        public async Task<string> AddImageAsync([NotNull] Stream stream, CancellationToken cancellationToken = default)
        {
            var displayName = Guid.NewGuid().ToString("N");
            var mimeType = await GetMimeTypeAsync(stream);
            var values = new ContentValues();

            // values.Put(MediaStore.IMediaColumns.DocumentId, bookId);
            values.Put(MediaStore.IMediaColumns.DisplayName, displayName);
            values.Put(MediaStore.IMediaColumns.MimeType, mimeType);
            values.Put(MediaStore.IMediaColumns.IsPending, true);

            var contentUri = resolver.Insert(collectionUri, values);

            using (var afd = resolver.OpenAssetFileDescriptor(contentUri, WriteMode))
            {
                using (var output = afd.CreateOutputStream())
                {
                    await stream.CopyToAsync(output, cancellationToken);
                    await output.FlushAsync(cancellationToken);
                }
            }

            values.Clear();
            values.Put(MediaStore.IMediaColumns.IsPending, false);

            resolver.Update(contentUri, values, null);

            return contentUri.ToString();
        }

        [return: MaybeNull]
        public string AddImage([NotNull] Stream stream)
        {
            var displayName = Guid.NewGuid().ToString("N");
            var mimeType = GetMimeType(stream);
            var values = new ContentValues();

            values.Put(MediaStore.IMediaColumns.DisplayName, displayName);
            values.Put(MediaStore.IMediaColumns.MimeType, mimeType);
            values.Put(MediaStore.IMediaColumns.IsPending, true);

            var contentUri = resolver.Insert(collectionUri, values);

            if (null == contentUri)
            {
                return null;
            }

            using (var descriptor = resolver.OpenAssetFileDescriptor(contentUri, WriteMode))
            {
                if (null == descriptor)
                {
                    throw new global::System.Exception();
                }

                using (var output = descriptor.CreateOutputStream())
                {
                    if (null == output)
                    {
                        throw new global::System.Exception();
                    }

                    stream.CopyTo(output);
                    output.Flush();
                }
            }

            values.Clear();
            values.Put(MediaStore.IMediaColumns.IsPending, false);

            resolver.Update(contentUri, values, null);

            return contentUri.ToString();
        }

        public void RemoveImage([NotNull] string contentUri)
        {
            var uri = Uri.Parse(contentUri);
            //var descriptor = resolver.OpenAssetFileDescriptor(uri, WriteMode);
            resolver.Delete(uri, Bundle.Empty);
        }

        public Task<Stream> GetImageAsync([NotNull] string contentUri, CancellationToken cancellationToken = default)
        {
            var uri = Uri.Parse(contentUri);
            var afd = resolver.OpenAssetFileDescriptor(uri, ReadMode);

            return Task.FromResult(afd.CreateInputStream());
        }

        [return: MaybeNull]
        public Stream GetImage([NotNull] string contentUri)
        {
            if (null == contentUri)
            {
                throw new ArgumentNullException(nameof(contentUri));
            }

            var uri = Uri.Parse(contentUri);
            var descriptor = resolver.OpenAssetFileDescriptor(uri, ReadMode);

            return descriptor?.CreateInputStream();
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

        private static string GetMimeType(Stream stream)
        {
            var dict = new[]
            {
                new KeyValuePair<byte[], string>(new byte[] {0xFF, 0xD8, 0xFF }, "image/jpeg"),
                new KeyValuePair<byte[], string>(new byte[] {0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A }, "image/png"),
                new KeyValuePair<byte[], string>(new byte[] {0x47, 0x49, 0x46 }, "image/gif")
            };
            
            var buffer = new byte[8];
            var count = stream.Read(buffer, 0, buffer.Length);
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