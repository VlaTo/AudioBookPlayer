using System.Collections.Generic;
using AudioBookPlayer.Data.Persistence.Entities;
using AudioBookPlayer.Domain.Models;

namespace AudioBookPlayer.Data.Persistence.Builders
{
    internal sealed class BookBuilder
    {
        public Book CreateBook(AudioBook source)
        {
            var book = new Book
            {
                Title = source.Title,
                Description = source.Description,
                Created = source.Created
            };

            MapAuthors(book, source.Authors);
            MapChapters(book, source.Sections);

            return book;
        }

        private static void MapAuthors(Book book, IReadOnlyList<AudioBookAuthor> authors)
        {
            var bookAuthors = new Author[authors.Count];

            for (var index = 0; index < authors.Count; index++)
            {
                bookAuthors[index] = new Author
                {
                    Name = authors[index].Name
                };
            }

            book.Authors = bookAuthors;
        }

        private void MapChapters(Book book, IReadOnlyList<AudioBookSection> sections)
        {
            book.Sections = new Section[sections.Count];
            var order = 0;

            for (var sectionIndex = 0; sectionIndex < sections.Count; sectionIndex++)
            {
                var audioBookSection = sections[sectionIndex];
                var section = new Section
                {
                    Title = audioBookSection.Title,
                    //ContentUri = audioBookSection.
                    Chapters = new Chapter[audioBookSection.Chapters.Count]
                };

                book.Sections[sectionIndex] = section;

                for (var chapterIndex = 0; chapterIndex < audioBookSection.Chapters.Count; chapterIndex++)
                {
                    var audioBookChapter = audioBookSection.Chapters[chapterIndex];
                    var chapter = new Chapter
                    {
                        Order = order,
                        Title = audioBookChapter.Title,
                        Offset = audioBookChapter.Offset,
                        Duration = audioBookChapter.Duration
                    };

                    section.Chapters[chapterIndex] = chapter;
                    order++;
                }
            }
        }
    }
}