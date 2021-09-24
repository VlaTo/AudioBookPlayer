using System.Collections.Generic;

namespace AudioBookPlayer.App.Domain.Models
{
    public sealed class AudioBookSection
    {
        public AudioBook AudioBook
        {
            get;
        }

        public string ContentUri
        {
            get;
            set;
        }

        public string Name
        {
            get;
            set;
        }

        public int Index
        {
            get
            {
                if (null != AudioBook)
                {
                    for (var index = 0; index < AudioBook.Sections.Count; index++)
                    {
                        var section = AudioBook.Sections[index];

                        if (ReferenceEquals(section, this))
                        {
                            return index;
                        }
                    }
                }

                return -1;
            }
        }
        
        public IList<AudioBookChapter> Chapters
        {
            get;
        }

        public AudioBookSection(AudioBook audioBook)
        {
            Chapters = new List<AudioBookChapter>();
            AudioBook = audioBook;
        }
    }
}