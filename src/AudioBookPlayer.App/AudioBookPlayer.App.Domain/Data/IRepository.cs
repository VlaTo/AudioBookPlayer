using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace AudioBookPlayer.App.Domain.Data
{
    public interface IRepository<T>
        where T : IEntity
    {
        Task AddAsync(T model);

        Task<T> GetAsync(long id);

        Task RemoveAsync(T model);

        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
    }
}