using System;
using System.Collections.Generic;
using System.Text;

namespace AudioBookPlayer.App.Models
{
    public sealed class AudioBook
    {
        public Guid Id
        {
            get;
        }

        public string Title
        {
            get;
        }

        public AudioBook(Guid id)
        {
            Id = id;
        }
    }
}
