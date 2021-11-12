using Android.App;
using Android.Content;
using Android.OS;
using Android.Provider;
using AudioBookPlayer.App.Android.Core;
using AudioBookPlayer.App.Domain.Models;
using AudioBookPlayer.App.Services;
using LibraProgramming.Media.Common;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using AudioBookPlayer.App.Domain.Extensions;
using AudioBookPlayer.App.Domain.Providers;
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

        /*public IReadOnlyList<AudioBook> QueryBooks()
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

                        using (var stream = new BufferedStream(descriptor.CreateInputStream(), 20480))
                        {
                            var mediaInfo = provider.ExtractMediaInfo(stream);
                            var audioBook = GetOrCreateAudioBook(audioBooks, audioFile, mediaInfo);
                            var sourceFiles = new Dictionary<string, TimeSpan>();

                            foreach (var track in mediaInfo.Tracks)
                            {
                                var contentUri = audioFile.ContentUri.ToString();
                                var part = audioBook.GetOrCreatePart(audioFile.Title);
                                var chapter = new AudioBookChapter(audioBook, track.Title, audioBook.GetDuration(), part);
                                var sourceFile = new AudioBookSourceFile(audioBook, contentUri);

                                if (null != part)
                                {
                                    part.Chapters.Add(chapter);
                                }

                                if (false == sourceFiles.TryGetValue(contentUri, out var duration))
                                {
                                    duration = TimeSpan.Zero;
                                }

                                var fragment = new AudioBookChapterFragment(duration, track.Duration, sourceFile);

                                chapter.Fragments.Add(fragment);
                                audioBook.Chapters.Add(chapter);
                                audioBook.SourceFiles.Add(sourceFile);

                                sourceFiles[contentUri] = duration + track.Duration;
                            }

                            foreach (var kvp in mediaInfo.Meta)
                            {
                                if (String.Equals(WellKnownMediaTags.Cover, kvp.Key))
                                {
                                    for (var index = 0; index < kvp.Value.Count; index++)
                                    {
                                        if (kvp.Value[index] is MemoryValue item)
                                        {
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
        }*/

        public IReadOnlyList<AudioBook> QueryBooks(IProgress<float> progress)
        {
            var scanner = new AudioBookFileScanner(resolver, collectionUri);
            var audioFiles = scanner.QueryFiles();
            var total = audioFiles.Length;
            var count = 0.0f;

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

                        using (var stream = new BufferedStream(descriptor.CreateInputStream(), 20480))
                        {
                            var mediaInfo = provider.ExtractMediaInfo(stream);
                            var audioBook = GetOrCreateAudioBook(audioBooks, audioFile, mediaInfo);
                            var sourceFiles = new Dictionary<string, TimeSpan>();

                            foreach (var track in mediaInfo.Tracks)
                            {
                                var contentUri = audioFile.ContentUri.ToString();
                                var section = audioBook.GetOrCreatePart(audioFile.Title, contentUri);

                                if (false == sourceFiles.TryGetValue(contentUri, out var duration))
                                {
                                    duration = TimeSpan.Zero;
                                }

                                var chapter = new AudioBookChapter(audioBook, track.Title, section)
                                {
                                    Start = duration,
                                    Duration = track.Duration
                                };

                                section.Chapters.Add(chapter);

                                sourceFiles[contentUri] = duration + track.Duration;
                            }

                            foreach (var kvp in mediaInfo.Meta)
                            {
                                if (String.Equals(WellKnownMediaTags.Cover, kvp.Key))
                                {
                                    for (var index = 0; index < kvp.Value.Count; index++)
                                    {
                                        if (kvp.Value[index] is MemoryValue item)
                                        {
                                            var image = new StreamAudioBookImage(audioBook, item.Memory);
                                            audioBook.Images.Add(image);
                                        }
                                    }
                                }
                            }

                            audioBook.Duration = CalculateBookDuration(audioBook);
                        }
                    }

                    progress.Report(count++ / total);
                }
                catch (Exception exception)
                {
                    System.Diagnostics.Debug.WriteLine($"Error processing file: \"{audioFile.Name}\". {exception.Message}");
                }
            }

            return audioBooks;
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
                audioBook = new AudioBook(EntityId.Empty, audioFile.Album);
                audioBook.Authors.Add(new AudioBookAuthor(audioFile.Artist));
                audioBooks.Add(audioBook);
            }
            else
            {
                audioBook = audioBooks[audioBookIndex];
            }

            if (EntityId.Empty == audioBook.Id)
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

        private static TimeSpan CalculateBookDuration(AudioBook audioBook)
        {
            var duration = TimeSpan.Zero;

            for (var sectionIndex = 0; sectionIndex < audioBook.Sections.Count; sectionIndex++)
            {
                var section = audioBook.Sections[sectionIndex];

                for (var chapterIndex = 0; chapterIndex < section.Chapters.Count; chapterIndex++)
                {
                    var chapter = section.Chapters[chapterIndex];
                    duration += chapter.Duration;
                }
            }

            return duration;
        }
    }
}