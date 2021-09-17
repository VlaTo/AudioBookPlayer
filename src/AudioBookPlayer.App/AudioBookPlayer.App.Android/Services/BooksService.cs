using AudioBookPlayer.App.Domain.Models;
using AudioBookPlayer.App.Domain.Services;
using AudioBookPlayer.App.Persistence.LiteDb;
using AudioBookPlayer.App.Persistence.LiteDb.Models;
using System.Collections.Generic;

namespace AudioBookPlayer.App.Android.Services
{
    internal sealed class BooksService
    {
        private readonly LiteDbContext context;
        private readonly ICoverService coverService;

        public BooksService(LiteDbContext context, ICoverService coverService)
        {
            this.context = context;
            this.coverService = coverService;
        }

        public IReadOnlyList<Book> QueryBooks()
        {
            var collection = new List<Book>();

            using (var unitOfWork = new UnitOfWork(context, false))
            {
                var books = unitOfWork.Books.GetAll();
                collection.AddRange(books);
            }

            return collection.AsReadOnly();
        }

        public Book GetBook(BookId bookId)
        {
            var book = GetBookInternal((long)bookId);
            return book;
        }

        public void SaveBook(Book book)
        {
            SaveBookInternal(book);
        }

        private Book GetBookInternal(long bookId)
        {
            using (var unitOfWork = new UnitOfWork(context, false))
            {
                return unitOfWork.Books.Get(bookId);
            }
        }

        private void SaveBookInternal(Book book)
        {
            using (var unitOfWork = new UnitOfWork(context, false))
            {
                unitOfWork.Books.Add(book);
                unitOfWork.Commit();
            }
        }
    }
}