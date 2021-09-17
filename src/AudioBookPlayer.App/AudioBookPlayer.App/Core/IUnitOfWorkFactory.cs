using AudioBookPlayer.App.Domain.Data;
using AudioBookPlayer.App.Persistence.LiteDb;
using AudioBookPlayer.App.Persistence.LiteDb.Core;

namespace AudioBookPlayer.App.Core
{
    public interface IUnitOfWorkFactory
    {
        IUnitOfWork CreateUnitOfWork(bool useTransaction);
    }
}