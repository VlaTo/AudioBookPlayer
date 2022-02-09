using System.Collections.Generic;
using AudioBookPlayer.Data.Persistence.Entities;
using AudioBookPlayer.Domain;
using AudioBookPlayer.Domain.Models;
using AudioBookPlayer.Domain.Services;

namespace AudioBookPlayer.Data.Persistence.Builders
{
    internal sealed class BookBuilder
    {
        private readonly IImageService imageService;

        public BookBuilder(IImageService imageService)
        {
            this.imageService = imageService;
        }

        public Book CreateBook(AudioBook source)
        {
            var book = new Book
            {
                Id = source.Id.GetValueOrDefault(0L),
                BookId = source.MediaId,
                Title = source.Title,
                Description = source.Description,
                Duration = source.Duration,
                Created = source.Created,
                Hash = source.Hash
            };

            MapAuthors(book, source.Authors);
            MapChapters(book, source.Sections);
            MapImages(book, source.Images);

            return book;
        }

        private void MapImages(Book book, IReadOnlyList<IAudioBookImage> images)
        {
            var bookImages = new string[images.Count];

            for (var index = 0; index < images.Count; index++)
            {
                var image = images[index];

                if (image is IHasContentUri holder)
                {
                    bookImages[index] = holder.ContentUri;
                }
                else
                {
                    var contentUri = imageService.SaveImage(image.GetImageStream());
                    bookImages[index] = contentUri;
                }
            }

            book.Images = bookImages;
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

        private static void MapChapters(Book book, IReadOnlyList<AudioBookSection> sections)
        {
            book.Sections = new Section[sections.Count];
            var order = 0;

            for (var sectionIndex = 0; sectionIndex < sections.Count; sectionIndex++)
            {
                var audioBookSection = sections[sectionIndex];
                var section = new Section
                {
                    Title = audioBookSection.Title,
                    ContentUri = audioBookSection.SourceFileUri,
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