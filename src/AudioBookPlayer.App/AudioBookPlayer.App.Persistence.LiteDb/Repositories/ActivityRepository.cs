using AudioBookPlayer.App.Persistence.LiteDb.Models;
using LiteDB;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;

namespace AudioBookPlayer.App.Persistence.LiteDb.Repositories
{
    internal sealed class ActivityRepository : IActivityRepository
    {
        private readonly LiteDbContext context;

        public ActivityRepository(LiteDbContext context)
        {
            this.context = context;
        }

        public void Add([NotNull] Activity entity)
        {
            var activities = context.Activities();

            activities.EnsureIndex(activity => activity.MediaId);

            if (false == activities.Update(entity))
            {
                activities.Insert(entity);
            }
        }

        public Activity Get(long id)
        {
            var activities = context.Activities();

            activities.EnsureIndex(activity => activity.Id);

            var found = activities.FindById(new BsonValue(id));

            return found;
        }

        public void Remove(Activity entity)
        {
            var success = context.Activities().Delete(new BsonValue(entity.Id));
        }

        public IEnumerable<Activity> Find(Expression<Func<Activity, bool>> predicate)
        {
            var activities = context.Activities().Find(predicate);
            return activities;
        }

        [return: MaybeNull]
        public Activity GetRecent()
        {
            var activities = context.Activities();
            activities.EnsureIndex(activity => activity.Time);
            return activities.Query().OrderByDescending(activity => activity.Time).FirstOrDefault();
        }
    }
}