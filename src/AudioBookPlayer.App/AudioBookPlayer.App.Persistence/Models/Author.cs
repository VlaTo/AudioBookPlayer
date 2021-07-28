using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AudioBookPlayer.App.Persistence.Models
{
    [Table("authors")]
    public class Author
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id
        {
            get;
            set;
        }

        [Required, DataType(DataType.Text)]
        public string Name
        {
            get;
            set;
        }

        public ICollection<AuthorBook> AuthorBooks
        {
            get;
            set;
        }
    }
}