using AudioBookPlayer.App.Persistence.LiteDb;

namespace AudioBookPlayer.App.Core
{
    /// <summary>
    /// 
    /// </summary>
    public interface IUnitOfWorkFactory
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="useTransaction"></param>
        /// <returns></returns>
        IUnitOfWork CreateUnitOfWork(bool useTransaction);
    }
}