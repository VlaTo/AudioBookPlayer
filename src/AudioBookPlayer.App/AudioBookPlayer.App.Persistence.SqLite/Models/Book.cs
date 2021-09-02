﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AudioBookPlayer.App.Persistence.SqLite.Models
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

        public bool DoNotShow
        {
            get;
            set;
        }

        public ICollection<ChapterFragment> ChapterFragments
        {
            get;
            set;
        }

        public ICollection<Part> Parts
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