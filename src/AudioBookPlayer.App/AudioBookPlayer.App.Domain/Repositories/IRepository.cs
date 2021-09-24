using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using AudioBookPlayer.App.Domain.Core;

namespace AudioBookPlayer.App.Domain.Repositories
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TEntity"></typeparam>
    public interface IRepository<in TKey, TEntity>
        where TKey : struct
        where TEntity : IEntity
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        void Add(TEntity model);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        TEntity Get(TKey id);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        void Remove(TEntity model);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        IEnumerable<TEntity> Find(Expression<Func<TEntity, bool>> predicate);
    }
}