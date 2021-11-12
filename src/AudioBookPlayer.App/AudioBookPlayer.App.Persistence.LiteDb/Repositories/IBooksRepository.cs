using System.Collections.Generic;
using AudioBookPlayer.App.Domain.Repositories;
using AudioBookPlayer.App.Persistence.LiteDb.Models;

namespace AudioBookPlayer.App.Persistence.LiteDb.Repositories
{
    /// <summary>
    /// 
    /// </summary>
    public interface IBooksRepository : IRepository<long, Book>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        IReadOnlyCollection<Book> GetAll();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        int Count();
    }
}