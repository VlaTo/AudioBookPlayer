using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AudioBookPlayer.App.Persistence.Models
{
    [Table("chapters")]
    public sealed class Chapter
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id
        {
            get;
            set;
        }

        public int Position
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

        [Required]
        public long BookId
        {
            get;
            set;
        }

        public long PartId
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
        
        [ForeignKey(nameof(PartId))]
        public Part Part
        {
            get;
            set;
        }

        public ICollection<ChapterFragment> ChapterFragments
        {
            get;
            set;
        }
    }
}