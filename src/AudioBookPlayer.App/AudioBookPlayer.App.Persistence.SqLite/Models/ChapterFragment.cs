using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AudioBookPlayer.App.Persistence.SqLite.Models
{
    [Table("fragments")]
    public class ChapterFragment
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

        [Required]
        public long ChapterId
        {
            get;
            set;
        }

        public string ContentUri
        {
            get;
            set;
        }

        [DataType(DataType.Duration)]
        public TimeSpan Start
        {
            get;
            set;
        }

        [DataType(DataType.Duration)]
        public TimeSpan Length
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

        [ForeignKey(nameof(ChapterId))]
        public Chapter Chapter
        {
            get;
            set;
        }
    }
}