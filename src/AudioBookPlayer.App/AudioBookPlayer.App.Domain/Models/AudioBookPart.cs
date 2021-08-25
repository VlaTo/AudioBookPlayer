using System.Collections.Generic;

namespace AudioBookPlayer.App.Domain.Models
{
    public sealed class AudioBookPart
    {
        public AudioBook AudioBook
        {
            get;
        }

        public string Title
        {
            get;
        }
        
        public IList<AudioBookChapter> Chapters
        {
            get;
        }

        public AudioBookPart(AudioBook audioBook, string title)
        {
            Chapters = new List<AudioBookChapter>();
            AudioBook = audioBook;
            Title = title;
        }
    }
}