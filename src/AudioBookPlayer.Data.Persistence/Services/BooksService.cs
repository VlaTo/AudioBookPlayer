using AudioBookPlayer.Data.Persistence.Builders;
using AudioBookPlayer.Domain.Models;
using AudioBookPlayer.Domain.Services;
using System.Collections.Generic;
using System.Linq;

namespace AudioBookPlayer.Data.Persistence.Services
{
    public sealed class BooksService : IBooksService
    {
        private readonly LiteDbContext context;
        private readonly IImageService imageService;

        public BooksService(LiteDbContext context, IImageService imageService)
        {
            this.context = context;
            this.imageService = imageService;
        }

        public IReadOnlyList<AudioBook> QueryBooks()
        {
            var books = new List<AudioBook>();

            using (var scope = new UnitOfWork(context))
            {
                var builder = new AudioBookBuilder(imageService);
                var source = scope.Books.All();

                foreach (var book in source)
                {
                    var audioBook = builder.CreateAudioBook(book);
                    books.Add(audioBook);
                }
            }

            return books.AsReadOnly();
        }

        public void SaveBook(AudioBook audioBook)
        {
            var builder = new BookBuilder(imageService);

            using (var scope = new UnitOfWork(context))
            {
                var book = builder.CreateBook(audioBook);

                scope.Books.Add(book);
                scope.Commit();
            }
        }

        public bool UpdateBook(long id, AudioBook audioBook)
        {
            var builder = new BookBuilder(imageService);

            using (var scope = new UnitOfWork(context))
            {
                var source = scope.Books.Find(x => x.Id == id).FirstOrDefault();

                if (null != source)
                {
                    var book = builder.CreateBook(audioBook);
                    var result = scope.Books.Update(source.Id, book);
                    
                    scope.Commit();

                    return result;
                }
            }

            return false;
        }

        public bool RemoveBook(AudioBook audioBook)
        {
            using (var scope = new UnitOfWork(context))
            {
                if (audioBook.Id.HasValue)
                {
                    var result = scope.Books.Remove(audioBook.Id.Value);

                    scope.Commit();

                    return result;
                }
            }

            return false;
        }
    }
}