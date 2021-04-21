using System;
using System.Collections.Generic;
using System.Text;

namespace AudioBookPlayer.App.Models
{
    public sealed class AudioBook
    {
        public int Id
        {
            get;
        }

        public string Title
        {
            get;
            set;
        }

        public AudioBook(int id)
        {
            Id = id;
        }
    }
}
