﻿using AudioBookPlayer.App.Domain.Models;
using AudioBookPlayer.App.Services;
using LibraProgramming.Xamarin.Dependency.Container.Attributes;
using System.Collections.Generic;

namespace AudioBookPlayer.App.ViewModels
{
    internal sealed class AllBooksViewModel : BooksViewModelBase
    {
        [PrefferedConstructor]
        public AllBooksViewModel(IMediaBrowserServiceConnector browserServiceConnector)
            : base(browserServiceConnector)
        {
        }

        private void DoBindAudioBooks(IEnumerable<AudioBook> audioBooks)
        {
            Books.Clear();

            foreach (var audioBook in audioBooks)
            {
                /*foreach (var chapter in audioBook.Chapters)
                {
                    System.Diagnostics.Debug.WriteLine($"[DoBindAudioBooks] Chapter: \"{chapter.Title}\" from: {chapter.Start:g} end: {chapter.End:g} (duration: {chapter.Duration:g})");
                    for (var index = 0; index < chapter.Fragments.Count; index++)
                    {
                        var fragment = chapter.Fragments[index];
                        System.Diagnostics.Debug.WriteLine($"[DoBindAudioBooks]   [{index}]: \"{fragment.SourceFile.ContentUri}\" from: {fragment.Start:g} end: {fragment.End:g} (duration: {fragment.Duration:g})");
                        System.Diagnostics.Debug.WriteLine(null);
                    }
                }
                
                System.Diagnostics.Debug.WriteLine($"[DoBindAudioBooks] AudioBook duration: {audioBook.Duration:g}");*/

                var model = CreateAudioBookModel(audioBook);

                Books.Add(model);
            }
        }
    }
}
