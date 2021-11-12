using System;
using AudioBookPlayer.App.Persistence.LiteDb.Repositories;

namespace AudioBookPlayer.App.Persistence.LiteDb
{
    public interface IUnitOfWork : IDisposable
    {
        IBooksRepository Books
        {
            get;
        }

        IActivityRepository Activities
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