#nullable enable

using Android.App;
using Android.Content;
using Android.OS;
using Android.Provider;
using System;
using System.Collections.Generic;
using System.IO;
using Uri = Android.Net.Uri;

namespace AudioBookPlayer.Core
{
    public sealed class ImageContentService
    {
        private const string WriteMode = "w";
        private const string ReadMode = "r";

        public static ImageContentService? instance;

        private readonly Uri? collectionUri;
        private readonly ContentResolver? resolver;

        public static ImageContentService GetInstance()
        {
            if (null == instance)
            {
                instance = new ImageContentService();
            }

            return instance;
        }

        private ImageContentService()
        {
            collectionUri = GetExternalContentUri();
            resolver = Application.Context.ContentResolver;
        }

        public Stream GetImageStream(string? contentUri)
        {
            if (null == contentUri)
            {
                throw new ArgumentNullException(nameof(contentUri));
            }

            var uri = Uri.Parse(contentUri);

            if (null == uri)
            {
                throw new Exception();
            }

            return GetImageStream(uri);
        }

        public Stream GetImageStream(Uri contentUri)
        {
            if (null == resolver)
            {
                throw new Exception();
            }

            var descriptor = resolver.OpenAssetFileDescriptor(contentUri, ReadMode);

            if (null != descriptor)
            {
                var stream = descriptor.CreateInputStream();

                if (null != stream)
                {
                    return stream;
                }
            }

            throw new Exception();
        }

        public string? SaveImageStream(Stream stream)
        {
            var displayName = Guid.NewGuid().ToString("N");
            var mimeType = GetMimeType(stream);
            var values = new ContentValues();

            values.Put(MediaStore.IMediaColumns.DisplayName, displayName);
            values.Put(MediaStore.IMediaColumns.MimeType, mimeType);
            values.Put(MediaStore.IMediaColumns.IsPending, true);

            if (null == resolver || null == collectionUri)
            {
                throw new Exception();
            }

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

        private static Uri? GetExternalContentUri()
            => (Build.VERSION.SdkInt >= BuildVersionCodes.Q)
                ? MediaStore.Images.Media.GetContentUri(MediaStore.VolumeExternal)
                : MediaStore.Images.Media.ExternalContentUri;

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
            for (var index = 0; index < expected.Length; index++)
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