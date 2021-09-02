using System.ComponentModel.DataAnnotations.Schema;

namespace AudioBookPlayer.App.Persistence.SqLite.Models
{
    [Table("author-books")]
    public class AuthorBook
    {
        public long AuthorId
        {
            get;
            set;
        }

        public Author Author
        {
            get;
            set;
        }

        public long BookId
        {
            get;
            set;
        }

        public Book Book
        {
            get;
            set;
        }
    }
}