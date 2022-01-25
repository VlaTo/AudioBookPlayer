using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using AudioBookPlayer.Data.Persistence.Entities;

namespace AudioBookPlayer.Data.Persistence.Repositories
{
    public interface IRepository<in TKey, TEntity> where TKey : struct where TEntity : IEntity
    {
        void Add(TEntity entity);

        TEntity Get(TKey key);

        bool Remove(TEntity entity);

        IEnumerable<TEntity> Find(Expression<Func<TEntity, bool>> predicate);
    }
}