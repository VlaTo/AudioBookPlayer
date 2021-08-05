using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AudioBookPlayer.App.Core;
using AudioBookPlayer.App.Domain.Models;
using AudioBookPlayer.App.Domain.Services;

namespace AudioBookPlayer.App.Services
{
    internal sealed class BooksService : IBooksService
    {
        private readonly IUnitOfWorkFactory factory;

        public BooksService(IUnitOfWorkFactory factory)
        {
            this.factory = factory;
        }

        public Task<IReadOnlyList<AudioBook>> QueryBooksAsync(CancellationToken cancellationToken = default)
        {
            using (var unitOfWork = factory.CreateUnitOfWork(false))
            {
                return unitOfWork.Books.QueryBooksAsync(cancellationToken);
            }
        }

        public Task<AudioBook> GetAudioBookAsync(long bookId, CancellationToken cancellationToken = default)
        {
            using (var unitOfWork = factory.CreateUnitOfWork(false))
            {
                return unitOfWork.Books.GetAsync(bookId);
            }
        }
    }
}