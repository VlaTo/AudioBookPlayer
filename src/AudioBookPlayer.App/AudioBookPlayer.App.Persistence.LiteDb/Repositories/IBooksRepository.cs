using System.Collections.Generic;
using AudioBookPlayer.App.Persistence.LiteDb.Core;
using AudioBookPlayer.App.Persistence.LiteDb.Models;

namespace AudioBookPlayer.App.Persistence.LiteDb.Repositories
{
    public interface IBooksRepository : IRepository<long, Book>
    {
        IEnumerable<Book> GetAll();
    }
}