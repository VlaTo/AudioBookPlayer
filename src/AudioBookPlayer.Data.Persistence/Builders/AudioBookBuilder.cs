using AudioBookPlayer.Data.Persistence.Entities;
using AudioBookPlayer.Domain.Models;
using System;
using System.Linq;

namespace AudioBookPlayer.Data.Persistence.Builders
{
    internal sealed class AudioBookBuilder
    {
        public AudioBook CreateAudioBook(Book source)
        {
            var book = new AudioBook
            {
                Id = source.Id,
                Title = source.Title,
                Description = source.Description,
                Created = source.Created,
                Duration = source.Duration
            };

            MapAuthors(book, source.Authors);
            MapChapters(book, source.Sections);

            return book;
        }

        private static void MapAuthors(AudioBook audioBook, Author[] authors)
        {
            var audioBookAuthors = new AudioBookAuthor[authors.Length];

            for (var index = 0; index < authors.Length; index++)
            {
                audioBookAuthors[index] = new AudioBookAuthor(audioBook, authors[index].Name);
            }

            audioBook.Authors = audioBookAuthors;
        }

        private static void MapChapters(AudioBook audioBook, Section[] sections)
        {
            var count = sections.SelectMany(x => x.Chapters).Count();
            var audioBookChapters = new AudioBookChapter[count];
            var audioBookSections = new AudioBookSection[sections.Length];

            for (var sectionIndex = 0; sectionIndex < sections.Length; sectionIndex++)
            {
                var section = sections[sectionIndex];
                var chapters = new AudioBookChapter[section.Chapters.Length];
                var audioBookSection = new AudioBookSection(audioBook)
                {
                    Title = section.Title,
                    Chapters = chapters
                };

                for (var chapterIndex = 0; chapterIndex < section.Chapters.Length; chapterIndex++)
                {
                    var chapter = section.Chapters[chapterIndex];
                    var audioBookChapter = new AudioBookChapter(audioBook, audioBookSection)
                    {
                        Title = chapter.Title,
                        Offset = chapter.Offset,
                        Duration = chapter.Duration,
                        Fragments = Array.Empty<AudioBookFragment>()
                    };

                    audioBookChapters[chapter.Order] = audioBookChapter;
                    chapters[chapterIndex] = audioBookChapter;
                }

                audioBookSections[sectionIndex] = audioBookSection;
            }

            audioBook.Sections = audioBookSections;
            audioBook.Chapters = audioBookChapters;
        }
    }
}