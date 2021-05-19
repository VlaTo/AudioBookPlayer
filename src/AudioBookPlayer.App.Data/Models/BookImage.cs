using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AudioBookPlayer.App.Data.Models
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
        [DataType(DataType.Text)]
        public string Name
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

        public byte[] Blob
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