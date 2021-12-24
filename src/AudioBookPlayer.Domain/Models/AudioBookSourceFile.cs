namespace AudioBookPlayer.Domain.Models
{
    public sealed class AudioBookSourceFile
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

        public AudioBookSourceFile(AudioBook audioBook)
        {
            AudioBook = audioBook;
        }
    }
}