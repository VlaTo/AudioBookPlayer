using Android.App;
using Android.OS;
using Android.Provider;
using AudioBookPlayer.App.Android.Core;
using AudioBookPlayer.App.Models;
using AudioBookPlayer.App.Services;
using LibraProgramming.Media.Common;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Uri = Android.Net.Uri;

namespace AudioBookPlayer.App.Android.Services
{
    internal sealed class BooksProvider : IBooksProvider
    {
        private readonly IAudioBookFactoryProvider factoryProvider;

        public BooksProvider(IAudioBookFactoryProvider factoryProvider)
        {
            this.factoryProvider = factoryProvider;
        }

        public Task<IReadOnlyList<AudioBook>> QueryBooksAsync(CancellationToken cancellationToken = default)
        {
            var collection = GetExternalContentUri();
            var contentResolver = Application.Context.ContentResolver;
            var scanner = new AudioBookFileScanner(contentResolver, collection);
            var audioFiles = scanner.QueryFiles();
            
            var audioBooks = new List<AudioBook>();

            foreach (var audioFile in audioFiles)
            {
                var (mimeType, mimeInfo) = scanner.GetMimeInfo(audioFile);

                try
                {
                    using (var descriptor = scanner.OpenFile(audioFile))
                    {
                        var extension = Path.GetExtension(audioFile.Name);
                        var factory = factoryProvider.CreateFactoryFor(extension);

                        using (var stream = descriptor.CreateInputStream())
                        {
                            var mediaInfo = factory.ExtractMediaInfo(stream);
                            var audioBook = GetOrCreateAudioBook(audioBooks, audioFile, mediaInfo);

                            foreach (var track in mediaInfo.Tracks)
                            {
                                var chapter = new AudioBookChapter(audioBook, track.Title, track.Duration);
                                var sourceFile = new AudioBookSourceFile(audioBook, audioFile.ContentUri.ToString(), descriptor.Length);

                                chapter.Fragments.Add(new AudioBookChapterFragment(audioBook.Duration, track.Duration, sourceFile));
                                audioBook.Chapters.Add(chapter);
                                audioBook.SourceFiles.Add(sourceFile);
                            }
                        }
                    }
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception);
                    throw;
                }
            }

            return Task.FromResult<IReadOnlyList<AudioBook>>(audioBooks);
        }

        private static Uri GetExternalContentUri()
        {
            return (Build.VERSION.SdkInt >= BuildVersionCodes.Q)
                ? MediaStore.Audio.Media.GetContentUri(MediaStore.VolumeExternal)
                : MediaStore.Audio.Media.ExternalContentUri;
        }

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
                //audioBook.Id = 1;

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