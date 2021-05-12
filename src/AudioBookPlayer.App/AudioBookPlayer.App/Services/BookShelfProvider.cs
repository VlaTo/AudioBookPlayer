using AudioBookPlayer.App.Core;
using AudioBookPlayer.App.Data;
using AudioBookPlayer.App.Models;
using LibraProgramming.Xamarin.Dependency.Container.Attributes;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using LibraProgramming.Media.Common;
using LibraProgramming.Media.QuickTime;
using Xamarin.Essentials;

namespace AudioBookPlayer.App.Services
{
    internal sealed class BookShelfProvider : IBookShelfProvider
    {
        private static readonly string[] SupportedExtensions = { ".mp3", ".m4b" };

        private readonly IPermissionRequestor permissionRequestor;
        private readonly IBookShelfDataContext context;
        private readonly ApplicationSettings settings;
        private readonly WeakEventManager<AudioBooksEventArgs> queryBooksReady;

        public event EventHandler<AudioBooksEventArgs> QueryBooksReady
        {
            add => queryBooksReady.AddEventHandler(value);
            remove => queryBooksReady.RemoveEventHandler(value);
        }

        [PrefferedConstructor]
        public BookShelfProvider(
            ApplicationSettings settings,
            IBookShelfDataContext context,
            IPermissionRequestor permissionRequestor)
        {
            this.settings = settings;
            this.context = context;
            this.permissionRequestor = permissionRequestor;

            queryBooksReady = new WeakEventManager<AudioBooksEventArgs>();
        }

        public async Task QueryBooksAsync()
        {
            var books = await context.Books
                .Where(book => !book.IsExcluded)
                .Select(book => new AudioBook(book.Id)
                {
                    Title = book.Title
                })
                .AsNoTracking()
                .ToArrayAsync();

            queryBooksReady.RaiseEvent(this, new AudioBooksEventArgs(books), nameof(QueryBooksReady));
        }

        public async Task RefreshBooksAsync()
        {
            var status = await permissionRequestor.CheckAndRequestMediaPermissionsAsync();

            if (PermissionStatus.Denied == status)
            {
                return;
            }

            var path = settings.LibraryRootPath;

            EnumerateBooks(path, 0);
            
            await QueryBooksAsync();
        }

        private void EnumerateBooks(string path, int level)
        {
            if (!Directory.Exists(path))
            {
                return;
            }

            // enumerate files
            foreach (var file in Directory.EnumerateFiles(path))
            {
                var folder = Path.GetDirectoryName(file);
                var filename = Path.GetFileName(file);
                var ext = Path.GetExtension(filename);

                if (IsSupportedExtension(ext))
                {
                    ExtractMediaInfo(file, filename, folder);
                }
            }

            // enumerate sub-folders
            var next = level + 1;

            foreach (var directory in Directory.EnumerateDirectories(path))
            {
                EnumerateBooks(directory, next);
            }
        }

        private static void ExtractMediaInfo(string file, string filename, string folder)
        {
            using (var stream = File.Open(file, FileMode.Open, FileAccess.Read))
            {
                using (var extractor = QuickTimeMediaExtractor.CreateFrom(stream))
                {
                    var audioBook = new AudioBook(-1);
                    var meta = extractor.GetMeta();
                    var tracks = extractor.GetTracks();

                    foreach (var item in meta.Items)
                    {
                        switch (item)
                        {
                            case MetaInformationTextItem textItem:
                            {
                                if (WellKnownMetaItemNames.Title.Equals(textItem.Key))
                                {
                                    audioBook.Title = textItem.Text;
                                }

                                if (WellKnownMetaItemNames.Author.Equals(textItem.Key))
                                {
                                    audioBook.Author = textItem.Text;
                                }

                                break;
                            }

                            case MetaInformationStreamItem streamItem:
                            {
                                if (WellKnownMetaItemNames.Cover.Equals(streamItem.Key))
                                {
                                    ;
                                }

                                break;
                            }
                        }
                    }

                    var total = TimeSpan.Zero;

                    foreach (var track in tracks)
                    {
                        audioBook.Tracks.Add(new )
                        //Console.WriteLine($"[Track] '{track.Title}' {track.Duration:hh':'mm':'ss}");
                        total += track.Duration;
                    }
                }
            }

            Debug.WriteLine($"[BookShelfProvider] [EnumerateBooks] File: \"{filename}\" in \"{folder}\"");
        }

        private static bool IsSupportedExtension(string ext)
        {
            return Array.Exists(
                SupportedExtensions,
                actual => String.Equals(actual, ext, StringComparison.InvariantCultureIgnoreCase)
            );
        }
    }
}
