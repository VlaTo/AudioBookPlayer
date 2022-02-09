using AudioBookPlayer.Data.Persistence.Entities;
using AudioBookPlayer.Domain.Models;
using System;
using System.Linq;
using AudioBookPlayer.Domain.Services;

namespace AudioBookPlayer.Data.Persistence.Builders
{
    internal sealed class AudioBookBuilder
    {
        private readonly IImageService imageService;

        public AudioBookBuilder(IImageService imageService)
        {
            this.imageService = imageService;
        }

        public AudioBook CreateAudioBook(Book source)
        {
            var book = new AudioBook
            {
                Id = source.Id,
                MediaId = source.BookId,
                Title = source.Title,
                Description = source.Description,
                Created = source.Created,
                Duration = source.Duration,
                Hash = source.Hash
            };

            MapAuthors(book, source.Authors);
            MapChapters(book, source.Sections);
            MapImages(book, source.Images);

            return book;
        }

        private void MapImages(AudioBook audioBook, string[] images)
        {
            var bookImages = new IAudioBookImage[images.Length];

            for (var index = 0; index < images.Length; index++)
            {
                bookImages[index] = new ContentProviderImage(audioBook, imageService, images[index]);
            }

            audioBook.Images = bookImages;
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
                    SourceFileUri = section.ContentUri,
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
                        // Fragments = Array.Empty<AudioBookFragment>()
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