using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AudioBookPlayer.App.Data.Models
{
    [Table("sources")]
    public class SourceFile
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
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

        public string Source
        {
            get;
            set;
        }

        [DataType(DataType.DateTime)]
        public DateTime Created
        {
            get;
            set;
        }

        [DataType(DataType.DateTime)]
        public DateTime Modified
        {
            get;
            set;
        }

        public long Length
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