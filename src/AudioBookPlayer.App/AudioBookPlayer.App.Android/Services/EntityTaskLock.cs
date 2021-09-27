using System.Collections.Generic;
using System.Threading.Tasks;
using AudioBookPlayer.App.Domain.Models;

namespace AudioBookPlayer.App.Android.Services
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    internal sealed class EntityTaskLock<TEntity> : ITaskLock<EntityId, TEntity>
    {
        private readonly Dictionary<EntityId, TaskCompletionSource<TEntity>> completionSources;
        private readonly object gate;

        public EntityTaskLock()
        {
            gate = new object();
            completionSources = new Dictionary<EntityId, TaskCompletionSource<TEntity>>();
        }

        public bool Acquire(EntityId key, TaskCompletionSource<TEntity> comparand, out TaskCompletionSource<TEntity> current)
        {
            lock (gate)
            {
                if (false == completionSources.TryGetValue(key, out var value))
                {
                    completionSources.Add(key, comparand);
                    current = comparand;

                    return true;
                }

                current = value;

                return false;
            }
        }

        public void Release(EntityId key, TaskCompletionSource<TEntity> current)
        {
            lock (gate)
            {
                if (completionSources.TryGetValue(key, out var value) && current == value)
                {
                    completionSources.Remove(key);
                }
            }
        }
    }
}