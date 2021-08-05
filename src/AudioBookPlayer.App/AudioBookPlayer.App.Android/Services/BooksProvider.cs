using Android.App;
using Android.Content;
using Android.OS;
using Android.Provider;
using AudioBookPlayer.App.Android.Core;
using AudioBookPlayer.App.Domain.Data;
using AudioBookPlayer.App.Domain.Models;
using AudioBookPlayer.App.Services;
using LibraProgramming.Media.Common;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using AudioBookPlayer.App.Models;
using Path = System.IO.Path;
using Uri = Android.Net.Uri;

namespace AudioBookPlayer.App.Android.Services
{
    internal sealed class BooksProvider : IBooksProvider
    {
        private readonly IMediaInfoProviderFactory factory;
        private readonly Uri collectionUri;
        private readonly ContentResolver resolver;

        public BooksProvider(IMediaInfoProviderFactory factory)
        {
            this.factory = factory;
            
            collectionUri = GetExternalContentUri();
            resolver = Application.Context.ContentResolver;
        }

        public Task<IReadOnlyList<AudioBook>> QueryBooksAsync(CancellationToken cancellationToken = default)
        {
            var scanner = new AudioBookFileScanner(resolver, collectionUri);
            var audioFiles = scanner.QueryFiles();
            
            var audioBooks = new List<AudioBook>();

            foreach (var audioFile in audioFiles)
            {
                var (mimeType, _) = scanner.GetMimeInfo(audioFile);

                try
                {
                    using (var descriptor = scanner.OpenFile(audioFile))
                    {
                        var extension = Path.GetExtension(audioFile.Name);
                        var provider = factory.CreateProviderFor(extension, mimeType);

                        if (null == provider)
                        {
                            System.Diagnostics.Debug.WriteLine($"No provider for file \"{audioFile.Name}\" with type \"{mimeType}\"");
                            continue;
                        }

                        using (var stream = descriptor.CreateInputStream())
                        {
                            var mediaInfo = provider.ExtractMediaInfo(stream);
                            var audioBook = GetOrCreateAudioBook(audioBooks, audioFile, mediaInfo);

                            foreach (var track in mediaInfo.Tracks)
                            {
                                var chapter = new AudioBookChapter(audioBook, track.Title, audioBook.Duration);
                                var sourceFile = new AudioBookSourceFile(audioBook, audioFile.ContentUri.ToString());

                                var fragment = new AudioBookChapterFragment(audioBook.Duration, track.Duration, sourceFile);

                                chapter.Fragments.Add(fragment);
                                audioBook.Chapters.Add(chapter);
                                audioBook.SourceFiles.Add(sourceFile);
                            }

                            foreach (var kvp in mediaInfo.Meta)
                            {
                                if (String.Equals(WellKnownMediaTags.Cover, kvp.Key))
                                {
                                    for (var index = 0; index < kvp.Value.Count; index++)
                                    {
                                        if (kvp.Value[index] is MemoryValue item)
                                        {
                                            /*using (var input = item.Memory.AsStream())
                                            {
                                                var bitmap = await BitmapFactory.DecodeStreamAsync(input);
                                                var bi = bitmap.GetBitmapInfo();
                                                //MimeTypeMap.Singleton.
                                                //bi.Format
                                            }*/

                                            var key = Guid.NewGuid().ToString("N");
                                            var image = new StreamAudioBookImage(audioBook, item.Memory);
                                            
                                            audioBook.Images.Add(image);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception exception)
                {
                    System.Diagnostics.Debug.WriteLine($"Error processing file: \"{audioFile.Name}\". {exception.Message}");
                }
            }

            return Task.FromResult<IReadOnlyList<AudioBook>>(audioBooks);
        }

        private static Uri GetExternalContentUri()
            => (Build.VERSION.SdkInt >= BuildVersionCodes.Q)
                ? MediaStore.Audio.Media.GetContentUri(MediaStore.VolumeExternal)
                : MediaStore.Audio.Media.ExternalContentUri;

        private static AudioBook GetOrCreateAudioBook(
            List<AudioBook> audioBooks,
            AudioBookFile audioFile,
            MediaInfo info)
        {
            var audioBookIndex = audioBooks.FindIndex(book => String.Equals(book.Title, audioFile.Album));
            AudioBook audioBook;

            if (0 > audioBookIndex)
            {
                audioBook = new AudioBook(audioFile.Album);
                audioBook.Authors.Add(new AudioBookAuthor(audioFile.Artist));
                audioBooks.Add(audioBook);
            }
            else
            {
                audioBook = audioBooks[audioBookIndex];
            }

            if (false == audioBook.Id.HasValue)
            {
                if (String.IsNullOrEmpty(audioBook.Synopsis))
                {
                    foreach (var (key, tags) in info.Meta)
                    {
                        switch (key)
                        {
                            case WellKnownMediaTags.Subtitle:
                            {
                                UpdateBookSynopsis(audioBook, tags);
                                break;
                            }

                            case WellKnownMediaTags.Cover:
                            {
                                foreach (var item in tags)
                                {
                                    if (item is StreamValue value)
                                    {
                                        // streamItem.Stream
                                    }
                                }

                                break;
                            }
                        }
                    }
                }
            }

            return audioBook;
        }

        private static void UpdateBookSynopsis(AudioBook audioBook, TagsCollection tags)
        {
            var synopsis = new List<string>();

            foreach (var item in tags)
            {
                if (item is TextValue value)
                {
                    synopsis.Add(value.Text);
                }
            }

            if (0 < synopsis.Count)
            {
                audioBook.Synopsis = String.Join(CultureInfo.CurrentUICulture.TextInfo.ListSeparator, synopsis);
            }
        }
    }
}