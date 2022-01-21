using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.OS;
using Android.Provider;
using AudioBookPlayer.Domain.Models;
using AudioBookPlayer.MediaBrowserService.Core.Extensions;
using AudioBookPlayer.MediaBrowserService.Core.Internal;
using LibraProgramming.Media.Common;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using AudioBookPlayer.Domain.Providers;
using Exception = Java.Lang.Exception;
using Uri = Android.Net.Uri;

namespace AudioBookPlayer.MediaBrowserService.Core
{
    internal sealed class BooksProvider : IBooksProvider
    {
        private readonly StringComparer comparer;
        private readonly ContentResolver contentResolver;
        private readonly Uri contentUri;
        private readonly MediaInfoProviderFactory factory;

        public BooksProvider()
        {
            comparer = StringComparer.CurrentCultureIgnoreCase;
            contentUri = GetExternalContentUri();
            contentResolver = Application.Context.ContentResolver;
            factory = new MediaInfoProviderFactory();
        }

        public IReadOnlyList<AudioBook> QueryBooks(/*IProgress<float> progress*/)
        {
            var scanner = new AudioBookFileScanner(contentResolver, contentUri);
            var files = scanner.QueryFiles();

            var audioBooks = new List<AudioBook>();
            var count = (float)files.Length;

            //progress.Report(0.0f);

            for (var index = 0; index < files.Length; index++)
            {
                var audioFile = files[index];
                var (mimeType, _) = scanner.GetMimeInfo(audioFile);

                ProcessAudioFile(scanner, audioBooks, audioFile, mimeType);

                //progress.Report((index + 1) / count);
            }

            //progress.Report(1.0f);

            return audioBooks.AsReadOnly();
        }

        private void ProcessAudioFile(AudioBookFileScanner scanner, List<AudioBook> audioBooks, AudioBookFile audioBookFile, string mimeType)
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
            catch (Exception e)
            {
                ;
            }
        }

        private void ScanAudioFile(MediaInfoProvider provider, List<AudioBook> audioBooks, AssetFileDescriptor descriptor, AudioBookFile audioBookFile)
        {
            var inputStream = descriptor.CreateInputStream();

            if (null == inputStream)
            {
                return ;
            }

            using (var stream = new BufferedStream(inputStream, 20480))
            {
                var mediaInfo = provider.ExtractMediaInfo(stream);
                var audioBook = GetOrCreateAudioBook(audioBooks, audioBookFile, mediaInfo);
                var sourceFiles = new Dictionary<string, TimeSpan>();

                ScanAudioTracks(sourceFiles, audioBookFile, mediaInfo, audioBook);
            }
        }

        private AudioBook GetOrCreateAudioBook(List<AudioBook> audioBooks, AudioBookFile audioFile, MediaInfo info)
        {
            var audioBook = audioBooks.Find(book => comparer.Equals(book.Title, audioFile.Album));

            if (null != audioBook)
            {
                return audioBook;
            }

            audioBook = new AudioBook
            {
                BookId = audioFile.Id,
                Title = audioFile.Album,
                Duration = TimeSpan.Zero
            };

            audioBook.Authors = new[]
            {
                new AudioBookAuthor(audioBook, audioFile.Artist)
            };

            foreach (var (key, tags) in info.Meta)
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
                        foreach (var item in tags)
                        {
                            if (item is StreamValue _)
                            {
                                // streamItem.Stream
                            }
                        }

                        break;
                    }
                }
            }

            audioBooks.Add(audioBook);

            return audioBook;
        }

        private void ScanAudioTracks(
            IDictionary<string, TimeSpan> sourceFiles,
            AudioBookFile audioBookFile,
            MediaInfo mediaInfo,
            AudioBook audioBook)
        {
            var sectionUri = audioBookFile.ContentUri.ToString();
            var section = GetOrCreateSection(audioBook, audioBookFile.Title);

            foreach (var track in mediaInfo.Tracks)
            {
                var chapter = new AudioBookChapter(audioBook, section)
                {
                    Title = track.Title,
                    Offset = audioBook.Duration,
                    Duration = track.Duration
                };

                //var chapter = new AudioBookChapter(audioBook, track.Title, bookBuilder.Duration, part);
                //var chapter = audioBook.CreateChapter(track.Title, part);
                var sourceFile = new AudioBookSourceFile(audioBook)
                {
                    ContentUri = sectionUri
                };

                if (false == sourceFiles.TryGetValue(sectionUri, out var duration))
                {
                    duration = TimeSpan.Zero;
                }

                var fragment = new AudioBookFragment(audioBook, sourceFile)
                {
                    Offset = duration,
                    Duration = track.Duration
                };

                chapter.Fragments = chapter.Fragments.Append(fragment);
                audioBook.Chapters = audioBook.Chapters.Append(chapter);
                audioBook.SourceFiles = audioBook.SourceFiles.Append(sourceFile);

                sourceFiles[sectionUri] = duration + track.Duration;

                audioBook.Duration += track.Duration;
            }
        }

        private AudioBookSection GetOrCreateSection(AudioBook audioBook, string title)
        {
            for (var index = 0; index < audioBook.Sections.Count; index++)
            {
                if (comparer.Equals(audioBook.Sections[index].Title, title))
                {
                    return audioBook.Sections[index];
                }
            }

            var section = new AudioBookSection(audioBook)
            {
                Title = title
            };

            audioBook.Sections = audioBook.Sections.Append(section);

            return section;
        }

        private static void UpdateBookDescription(AudioBook audioBook, TagsCollection tags)
        {
            var lines = new List<string>();

            foreach (var item in tags)
            {
                if (item is TextValue value)
                {
                    lines.Add(value.Text);
                }
            }

            audioBook.Description = 0 < lines.Count
                ? String.Join(CultureInfo.CurrentUICulture.TextInfo.ListSeparator, lines)
                : String.Empty;
        }

        private static Uri GetExternalContentUri() => (Build.VERSION.SdkInt >= BuildVersionCodes.Q)
            ? MediaStore.Audio.Media.GetContentUri(MediaStore.VolumeExternal)
            : MediaStore.Audio.Media.ExternalContentUri;

    }
}