using AndroidX.Room;

namespace AudioBookPlayer.Persistent.Entities
{
    [Entity(TableName = "books")]
    public class BookEntity
    {
        [PrimaryKey(AutoGenerate = true)]
        [ColumnInfo(Name = "rowid")]
        public long Id
        {
            get;
            set;
        }

        [ColumnInfo(Name = "title")]
        public string Title
        {
            get;
            set;
        }

        [ColumnInfo(Name = "description")]
        public string Description
        {
            get;
            set;
        }

        [ColumnInfo(Name = "reader")]
        public string Reader
        {
            get;
            set;
        }
    }
}