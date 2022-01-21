using System.Collections.Generic;
using AudioBookPlayer.Data.Persistence.Builders;
using AudioBookPlayer.Domain.Models;
using AudioBookPlayer.Domain.Services;

namespace AudioBookPlayer.Data.Persistence.Services
{
    public sealed class BooksService : IBooksService
    {
        private readonly LiteDbContext context;

        public BooksService(LiteDbContext context)
        {
            this.context = context;
        }

        public IReadOnlyList<AudioBook> QueryBooks()
        {
            var books = new List<AudioBook>();

            using (var scope = new UnitOfWork(context))
            {
                var builder = new AudioBookBuilder();
                var source = scope.Books.All();

                foreach (var book in source)
                {
                    var dto = builder.CreateAudioBook(book);
                    books.Add(dto);
                }
            }

            return books.AsReadOnly();
        }

        public void SaveBook(AudioBook audioBook)
        {
            var builder = new BookBuilder();

            using (var scope = new UnitOfWork(context))
            {
                var book = builder.CreateBook(audioBook);

                scope.Books.Add(book);
                scope.Commit();
            }
        }
    }
}