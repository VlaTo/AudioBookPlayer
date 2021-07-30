using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AudioBookPlayer.App.Persistence.Models
{
    [Table("book-images")]
    public class BookImage
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id
        {
            get; 
            set;
        }

        [Required]
        public long BookId
        {
            get;
            set;
        }

        [DataType(DataType.ImageUrl)]
        public string ContentUri
        {
            get;
            set;
        }

        [ForeignKey(nameof(BookId))]
        public Book Book
        {
            get;
            set;
        }
    }
}