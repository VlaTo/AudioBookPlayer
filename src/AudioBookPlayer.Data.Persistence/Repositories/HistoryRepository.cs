using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using AudioBookPlayer.Data.Persistence.Entities;

namespace AudioBookPlayer.Data.Persistence.Repositories
{
    public sealed class HistoryRepository : IHistoryRepository
    {
        private readonly LiteDbContext context;

        public HistoryRepository(LiteDbContext context)
        {
            this.context = context;
        }

        public void Add(History entity)
        {
            var collection = context.History();
            collection.Insert(entity);
        }

        public History Get(long key)
        {
            var collection = context.History();
            return collection.FindById(key);
        }

        public bool Remove(History entity)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<History> Find(Expression<Func<History, bool>> predicate)
        {
            throw new NotImplementedException();
        }
    }
}