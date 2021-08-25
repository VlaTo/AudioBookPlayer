using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AudioBookPlayer.App.Persistence.Models
{
    [Table("parts")]
    public class Part
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
        public string Title
        {
            get;
            set;
        }

        public long BookId
        {
            get;
            set;
        }

        public ICollection<Chapter> Chapters
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