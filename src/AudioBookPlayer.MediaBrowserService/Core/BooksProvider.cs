using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.OS;
using Android.Provider;
using AudioBookPlayer.Domain.Models;
using AudioBookPlayer.Domain.Providers;
using AudioBookPlayer.MediaBrowserService.Core.Internal;
using LibraProgramming.Media.Common;
using System.Collections.Generic;
using System.IO;
using Exception = Java.Lang.Exception;
using Uri = Android.Net.Uri;

namespace AudioBookPlayer.MediaBrowserService.Core
{
    internal sealed class BooksProvider : IBooksProvider
    {
        private readonly ContentResolver contentResolver;
        private readonly Uri contentUri;
        private readonly MediaInfoProviderFactory factory;

        public BooksProvider()
        {
            contentUri = GetExternalContentUri();
            contentResolver = Application.Context.ContentResolver;
            factory = new MediaInfoProviderFactory();
        }

        public IReadOnlyList<AudioBook> QueryBooks(/*IProgress<float> progress*/)
        {
            var scanner = new AudioBookFileScanner(contentResolver, contentUri);
            var files = scanner.QueryFiles();

            var audioBooks = new AudioBookList();
            //var count = (float)files.Length;

            //progress.Report(0.0f);

            for (var index = 0; index < files.Length; index++)
            {
                var audioFile = files[index];
                var (mimeType, _) = scanner.GetMimeInfo(audioFile);

                ProcessAudioFile(scanner, audioBooks, audioFile, mimeType);

                //progress.Report((index + 1) / count);
            }

            //progress.Report(1.0f);

            return audioBooks.Build();
        }

        private void ProcessAudioFile(AudioBookFileScanner scanner, AudioBookList audioBooks, AudioBookFile audioBookFile, string mimeType)
        {
            try
            {
                using (var descriptor = scanner.OpenFile(audioBookFile))
                {
                    var extension =  Path.GetExtension(audioBookFile.Name);
                    var provider = factory.CreateProviderFor(extension, mimeType);

                    if (null == provider)
                    {
                        System.Diagnostics.Debug.WriteLine($"No provider for file \"{audioBookFile.Name}\" with type \"{mimeType}\"");
                        return ;
                    }

                    ScanAudioFile(provider, audioBooks, descriptor, audioBookFile);
                }
            }
            catch (Exception exception)
            {
                ;
            }
        }

        private static void ScanAudioFile(MediaInfoProvider provider, AudioBookList audioBooks, AssetFileDescriptor descriptor, AudioBookFile audioBookFile)
        {
            var inputStream = descriptor.CreateInputStream();

            if (null == inputStream)
            {
                return ;
            }

            using (var stream = new BufferedStream(inputStream, 20480))
            {
                var mediaInfo = provider.ExtractMediaInfo(stream);
                var audioBook = audioBooks.GetAudioBook(audioBookFile.Album);

                if (null == audioBook)
                {
                    audioBook = audioBooks.NewAudioBook()
                        .SetMediaId(audioBookFile.Id)
                        .SetTitle(audioBookFile.Album)
                        .AddAuthor(audioBookFile.Artist);

                    foreach (var (key, tags) in mediaInfo.Meta)
                    {
                        switch (key)
                        {
                            case WellKnownMediaTags.Subtitle:
                            {
                                UpdateBookDescription(audioBook, tags);

                                break;
                            }

                            case WellKnownMediaTags.Cover:
                            {
                                UpdateBookImages(audioBook, tags);

                                break;
                            }
                        }
                    }
                }

                var audioBookSection = audioBook.NewSection()
                    .SetTitle(audioBookFile.Title)
                    .SetSourceFileUri(audioBookFile.ContentUri.ToString());

                ScanAudioTracks(audioBookSection, mediaInfo);
            }
        }

        private static void ScanAudioTracks(AudioBookSectionBuilder audioBookSection, MediaInfo mediaInfo)
        {
            foreach (var track in mediaInfo.Tracks)
            {
                audioBookSection.NewChapter()
                    .SetTitle(track.Title)
                    .SetDuration(track.Duration);
            }
        }

        private static void UpdateBookDescription(AudioBookBuilder audioBook, TagsCollection tags)
        {
            foreach (var item in tags)
            {
                if (item is TextValue value)
                {
                    audioBook.AddDescription(value.Text);
                    continue;
                }

                // skip non-text tags
                ;
            }
        }

        private static void UpdateBookImages(AudioBookBuilder audioBook, TagsCollection tags)
        {
            foreach (var item in tags)
            {
                if (item is MemoryValue memoryValue)
                {
                    audioBook.AddImage(memoryValue.Memory);
                    continue;
                }

                // skip non-stream tags
                ;
            }
        }

        private static Uri GetExternalContentUri() => (Build.VERSION.SdkInt >= BuildVersionCodes.Q)
            ? MediaStore.Audio.Media.GetContentUri(MediaStore.VolumeExternal)
            : MediaStore.Audio.Media.ExternalContentUri;
    }
}