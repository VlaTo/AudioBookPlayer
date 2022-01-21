namespace AudioBookPlayer.Domain.Models
{
    public sealed class AudioBookImage
    {
        public AudioBook AudioBook
        {
            get;
        }

        public string SourceFile
        {
            get;
        }

        public AudioBookImage(AudioBook audioBook, string sourceFile)
        {
            AudioBook = audioBook;
            SourceFile = sourceFile;
        }
    }
}