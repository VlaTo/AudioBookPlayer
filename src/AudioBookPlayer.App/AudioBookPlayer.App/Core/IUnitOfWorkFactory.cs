using AudioBookPlayer.App.Domain.Data;

namespace AudioBookPlayer.App.Core
{
    public interface IUnitOfWorkFactory
    {
        IUnitOfWork CreateUnitOfWork(bool useTransaction);
    }
}