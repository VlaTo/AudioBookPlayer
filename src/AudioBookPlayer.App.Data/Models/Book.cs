using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AudioBookPlayer.App.Data.Models
{
    [Table("books")]
    public class Book
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

        public ICollection<AuthorBook> AuthorBooks
        {
            get;
            set;
        }

        [DataType(DataType.MultilineText)]
        public string Synopsis
        {
            get;
            set;
        }

        [DataType(DataType.Duration)]
        public TimeSpan Duration
        {
            get;
            set;
        }

        [DataType(DataType.DateTime)]
        public DateTime AddedToLibrary
        {
            get;
            set;
        }

        public bool DoNotShow
        {
            get;
            set;
        }

        public ICollection<SourceFile> SourceFiles
        {
            get;
            set;
        }

        public ICollection<BookImage> Images
        {
            get;
            set;
        }

        public ICollection<Chapter> Chapters
        {
            get;
            set;
        }
    }
}
