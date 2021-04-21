using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AudioBookPlayer.App.Data.Models
{
    [Table("books")]
    public class Book
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id
        {
            get;
            set;
        }

        [Required, DataType(DataType.Text)]
        public string Title
        {
            get;
            set;
        }

        [DataType(DataType.MultilineText)]
        public string Description
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

        public bool IsExcluded
        {
            get;
            set;
        }

        public List<SourceFile> SourceFiles
        {
            get;
            set;
        }
    }
}
