using System.Collections.Generic;
using AudioBookPlayer.Data.Persistence.Entities;

namespace AudioBookPlayer.Data.Persistence.Repositories
{
    public interface IBooksRepository : IRepository<long, Book>
    {
        int Count();

        IEnumerable<Book> All();
    }
}