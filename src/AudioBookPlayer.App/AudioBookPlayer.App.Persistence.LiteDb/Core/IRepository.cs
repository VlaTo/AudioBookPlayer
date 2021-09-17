using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace AudioBookPlayer.App.Persistence.LiteDb.Core
{
    public interface IRepository<in TKey, TEntity>
        where TKey : struct
        where TEntity : IEntity
    {
        void Add(TEntity model);

        TEntity Get(TKey id);

        void Remove(TEntity model);

        IEnumerable<TEntity> Find(Expression<Func<TEntity, bool>> predicate);
    }
}