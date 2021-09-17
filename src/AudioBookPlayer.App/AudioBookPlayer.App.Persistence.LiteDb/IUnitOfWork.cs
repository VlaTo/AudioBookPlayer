using System;

namespace AudioBookPlayer.App.Persistence.LiteDb
{
    public interface IUnitOfWork : IDisposable
    {
        Repositories.IBooksRepository Books
        {
            get;
        }

        /*IBookmarkRepository Bookmarks
        {
            get;
        }*/

        void Commit();
    }
}