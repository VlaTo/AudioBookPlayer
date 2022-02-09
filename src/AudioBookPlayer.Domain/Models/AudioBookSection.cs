using System.Collections.Generic;

namespace AudioBookPlayer.Domain.Models
{
    public sealed class AudioBookSection
    {
        public AudioBook AudioBook
        {
            get;
        }

        public string Title
        {
            get;
            set;
        }

        public string SourceFileUri
        {
            get;
            set;
        }

        public IReadOnlyList<AudioBookChapter> Chapters
        {
            get;
            set;
        }

        public AudioBookSection(AudioBook audioBook)
        {
            AudioBook = audioBook;
            Chapters = new ArrayList<AudioBookChapter>();
        }
    }
}