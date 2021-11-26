using System.Collections.Generic;
using AndroidX.Room;
using AudioBookPlayer.Persistent.Entities;

namespace AudioBookPlayer.Persistent.DataAccess
{
    [Dao]
    public abstract class BookEntityDao
    {
        [Query(Value = "select * from [books]")]
        public abstract IList<BookEntity> GetAllBooks();

        [Query(Value = "select * from [books] where [rowid] = :id")]
        public abstract BookEntity GetBook(long id);

        [Insert(OnConflict = OnConflictStrategy.Replace)]
        public abstract void AddBook(BookEntity entity);
    }
}