using System;
using AudioBookPlayer.App.Domain.Core;
using AudioBookPlayer.App.Domain.Models;
using AudioBookPlayer.App.Domain.Services;
using AudioBookPlayer.App.Persistence.LiteDb;
using AudioBookPlayer.App.Persistence.LiteDb.Models;
using System.Collections.Generic;

namespace AudioBookPlayer.App.Android.Services
{
    internal sealed class BooksService : IBooksService
    {
        private readonly LiteDbContext context;
        private readonly ICoverService coverService;

        public BooksService(LiteDbContext context, ICoverService coverService)
        {
            this.context = context;
            this.coverService = coverService;
        }

        public bool IsEmpty()
        {
            using (var unitOfWork = new UnitOfWork(context, false))
            {
                return 0 == unitOfWork.Books.Count();
            }
        }

        public IReadOnlyList<AudioBook> QueryBooks(IProgress<float> progress)
        {
            var collection = new List<AudioBook>();

            using (var unitOfWork = new UnitOfWork(context, false))
            {
                var books = unitOfWork.Books.GetAll();
                var total = books.Count;
                var count = 0.0f;

                foreach (var book in books)
                {
                    var audioBook = CreateAudioBook(book);

                    collection.Add(audioBook);
                    progress.Report(count++ / total);
                }
            }

            return collection.AsReadOnly();
        }

        public AudioBook GetBook(EntityId bookId)
        {
            using (var unitOfWork = new UnitOfWork(context, false))
            {
                var book = unitOfWork.Books.Get((long)bookId);
                var audioBook = CreateAudioBook(book);

                return audioBook;
            }
        }

        public void SaveBook(AudioBook audioBook)
        {
            var book = CreateBook(audioBook);

            using (var unitOfWork = new UnitOfWork(context, false))
            {
                unitOfWork.Books.Add(book);
                unitOfWork.Commit();
            }
        }

        public void RemoveBook(AudioBook audioBook)
        {
            using (var unitOfWork = new UnitOfWork(context, false))
            {
                var book = unitOfWork.Books.Get((long)audioBook.Id);

                if (null != book)
                {
                    RemoveBookImages(book);

                    unitOfWork.Books.Remove(book);
                    unitOfWork.Commit();
                }
            }
        }

        private Book CreateBook(AudioBook audioBook)
        {
            var book = new Book
            {
                Id = (long)audioBook.Id,
                Title = audioBook.Title,
                Synopsis = audioBook.Synopsis,
                Duration = audioBook.Duration,
                Created = audioBook.Created,
                Authors = new string[audioBook.Authors.Count],
                Sections = new Section[audioBook.Sections.Count],
                Images = new string[audioBook.Images.Count]
            };

            // authors
            for (var index = 0; index < audioBook.Authors.Count; index++)
            {
                var author = audioBook.Authors[index];
                book.Authors[index] = author.Name;
            }

            // images
            for (var index = 0; index < audioBook.Images.Count; index++)
            {
                var image = audioBook.Images[index];
                string contentUri;

                if (image is IHasContentUri hcu)
                {
                    contentUri = hcu.ContentUri;
                }
                else
                {
                    contentUri = coverService.AddImage(image.GetStream());
                }

                book.Images[index] = contentUri;
            }

            // sections
            for (var sectionIndex = 0; sectionIndex < audioBook.Sections.Count; sectionIndex++)
            {
                var audioBookSection = audioBook.Sections[sectionIndex];
                var section = new Section
                {
                    Title = audioBookSection.Name,
                    ContentUri = audioBookSection.ContentUri,
                    Chapters = new Chapter[audioBookSection.Chapters.Count]
                };

                // chapters
                for (var chapterIndex = 0; chapterIndex < audioBookSection.Chapters.Count; chapterIndex++)
                {
                    var chapter = audioBookSection.Chapters[chapterIndex];
                    section.Chapters[chapterIndex] = new Chapter
                    {
                        Title = chapter.Title,
                        Start = chapter.Start,
                        Duration = chapter.Duration
                    };
                }

                book.Sections[sectionIndex] = section;
            }

            return book;
        }

        private AudioBook CreateAudioBook(Book book)
        {
            var audioBook = new AudioBook(book.Id, book.Title)
            {
                Synopsis = book.Synopsis,
                Created = book.Created,
                Duration = book.Duration
            };

            // authors
            for (var index = 0; index < book.Authors.Length; index++)
            {
                var name = book.Authors[index];
                audioBook.Authors.Add(new AudioBookAuthor(name));
            }

            // images
            for (var index = 0; index < book.Images.Length; index++)
            {
                var contentUri = book.Images[index];
                audioBook.Images.Add(new ContentProvidedAudioBookImage(audioBook, contentUri, coverService));
            }

            // sections
            for (var sectionIndex = 0; sectionIndex < book.Sections.Length; sectionIndex++)
            {
                var section = book.Sections[sectionIndex];
                var audioBookSection = new AudioBookSection(audioBook)
                {
                    Name = section.Title,
                    ContentUri = section.ContentUri
                };

                // chapters
                for (var chapterIndex = 0; chapterIndex < section.Chapters.Length; chapterIndex++)
                {
                    var chapter = section.Chapters[chapterIndex];
                    var audioBookChapter = new AudioBookChapter(audioBook, chapter.Title, audioBookSection)
                    {
                        Start = chapter.Start,
                        Duration = chapter.Duration
                    };

                    audioBookSection.Chapters.Add(audioBookChapter);
                }

                audioBook.Sections.Add(audioBookSection);
            }

            return audioBook;
        }

        private void RemoveBookImages(Book book)
        {
            for (var index = 0; index < book.Images.Length; index++)
            {
                var contentUri = book.Images[index];
                coverService.RemoveImage(contentUri);
            }
        }
    }
}