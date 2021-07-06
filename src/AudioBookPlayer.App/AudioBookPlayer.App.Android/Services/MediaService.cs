using Android.App;
using Android.OS;
using Android.Provider;
using AudioBookPlayer.App.Android.Core;
using AudioBookPlayer.App.Models;
using AudioBookPlayer.App.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Android.Media;
using Java.Nio.FileNio.Attributes;
using LibraProgramming.Media.Common;
using Environment = Android.OS.Environment;
using File = Java.IO.File;
using Uri = Android.Net.Uri;

namespace AudioBookPlayer.App.Android.Services
{
    internal sealed class MediaService : IMediaService
    {
        private readonly IAudioBookFactoryProvider audioBookFactoryProvider;

        public MediaService(IAudioBookFactoryProvider audioBookFactoryProvider)
        {
            this.audioBookFactoryProvider = audioBookFactoryProvider;
        }

        public Task<IEnumerable<AudioBook>> QueryBooksAsync()
        {
            var collection = (Build.VERSION.SdkInt >= BuildVersionCodes.Q)
                ? MediaStore.Audio.Media.GetContentUri(MediaStore.VolumeExternal)
                : MediaStore.Audio.Media.ExternalContentUri;
            var contentResolver = Application.Context.ContentResolver;
            var finder = new AudioBookFileScanner(contentResolver, collection);
            var audioFiles = finder.QueryFiles();
            
            var audioBooks = new List<AudioBook>();

            foreach (var audioFile in audioFiles)
            {
                try
                {
                    var streamType = contentResolver.GetType(audioFile.ContentUri);
                    var info = contentResolver.GetTypeInfo(streamType);

                    System.Diagnostics.Debug.WriteLine($"[QueryBooksAsync] Uri: '{audioFile.ContentUri}' mimetype: '{streamType}' lbl: '{info.Label}' desc: '{info.ContentDescription}'");

                    using (var descriptor = contentResolver.OpenAssetFileDescriptor(audioFile.ContentUri, "r"))
                    {
                        var extension = Path.GetExtension(audioFile.Name);
                        var factory = audioBookFactoryProvider.CreateFactoryFor(extension);

                        using (var stream = descriptor.CreateInputStream())
                        {
                            var information = factory.ExtractMediaInfo(stream);

                            if (false == SameBook(audioFile, information))
                            {
                                continue;
                            }

                            var audioBookIndex = audioBooks.FindIndex(book => String.Equals(book.Title, audioFile.Album));
                            AudioBook audioBook;

                            if (0 > audioBookIndex)
                            {
                                string synopsis = null;
                                
                                foreach (var item in information.Meta)
                                {
                                    switch (item.Key)
                                    {
                                        case WellKnownMetaItemNames.Subtitle:
                                        {
                                            if (item is MetaInformationTextItem textItem)
                                            {
                                                synopsis = textItem.Text;
                                            }

                                            break;
                                        }
                                    }
                                }

                                audioBook = new AudioBook(audioFile.Album, synopsis: synopsis);
                                audioBook.Authors.Add(new AudioBookAuthor(audioFile.Artist));
                                audioBooks.Add(audioBook);
                            }
                            else
                            {
                                audioBook = audioBooks[audioBookIndex];
                            }

                            foreach (var track in information.Tracks)
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

            return Task.FromResult<IEnumerable<AudioBook>>(audioBooks.ToArray());
        }

        private static bool SameBook(AudioBookFile audioFile, MediaInformation information)
        {
            if (String.Equals(audioFile.Album, information.BookTitle))
            {
                if (String.IsNullOrEmpty(audioFile.Artist))
                {
                    return false;
                }

                if (Array.Exists(information.BookAuthors, author => String.Equals(author, audioFile.Artist)))
                {
                    return true;
                }
            }

            return false;
        }
    }
}