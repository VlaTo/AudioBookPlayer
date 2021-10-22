﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace AudioBookPlayer.App.Domain.Models
{
    public sealed class AudioBook
    {
        public EntityId Id
        {
            get;
            set;
        }

        public string Title
        {
            get;
        }

        public string Synopsis
        {
            get;
            set;
        }

        public DateTime Created
        {
            get;
            set;
        }

        public TimeSpan Duration
        {
            get;
            set;
        }

        public List<AudioBookAuthor> Authors
        {
            get;
        }

        public List<AudioBookSection> Sections
        {
            get;
        }

        public List<AudioBookImage> Images
        {
            get;
        }

        public AudioBook(EntityId id, string title)
        {
            Id = id;
            Title = title;
            Authors = new List<AudioBookAuthor>();
            Sections = new List<AudioBookSection>();
            Images = new List<AudioBookImage>();
        }
    }
}
