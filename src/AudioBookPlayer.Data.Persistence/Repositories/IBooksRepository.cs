using System.Collections.Generic;
using AudioBookPlayer.Data.Persistence.Entities;

namespace AudioBookPlayer.Data.Persistence.Repositories
{
    public interface IBooksRepository : IRepository<long, Book>
    {
        bool Update(long key, Book book);

        bool Remove(long key);
        
        IEnumerable<Book> All();
    }
}