using AudioBookPlayer.App.Domain.Repositories;
using AudioBookPlayer.App.Persistence.LiteDb.Models;

namespace AudioBookPlayer.App.Persistence.LiteDb.Repositories
{
    public interface IActivityRepository : IRepository<long, Activity>
    {
        Activity GetRecent();
    }
}