using System;
using System.Collections.Generic;
using AudioBookPlayer.Domain.Models;

namespace AudioBookPlayer.MediaBrowserService.Core
{
    internal sealed class AudioBookList
    {
        private readonly List<AudioBookBuilder> books;

        public AudioBookList()
        {
            books = new List<AudioBookBuilder>();
        }

        public IReadOnlyList<AudioBook> Build()
        {
            var list = new List<AudioBook>();

            for (var index = 0; index < books.Count; index++)
            {
                var book = books[index].Build();
                list.Add(book);
            }

            return list.AsReadOnly();
        }

        public AudioBookBuilder GetAudioBook(string bookTitle)
        {
            for (var index = 0; index < books.Count; index++)
            {
                var actualTitle = books[index].Title;

                if (String.Equals(bookTitle, actualTitle, StringComparison.InvariantCultureIgnoreCase))
                {
                    return books[index];
                }
            }

            return null;
        }

        public AudioBookBuilder NewAudioBook()
        {
            var builder = AudioBookBuilder.Create();

            books.Add(builder);

            return builder;
        }
    }
}