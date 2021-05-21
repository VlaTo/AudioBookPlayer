using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AudioBookPlayer.App.Data.Models
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
        
        [Required]
        public long SourceFileId
        {
            get;
            set;
        }

        [DataType(DataType.Duration)]
        public TimeSpan Offset
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

        [ForeignKey(nameof(SourceFileId))]
        public SourceFile SourceFile
        {
            get;
            set;
        }
    }
}