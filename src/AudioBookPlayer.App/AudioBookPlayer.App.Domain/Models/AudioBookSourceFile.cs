namespace AudioBookPlayer.App.Domain.Models
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
        }

        public AudioBookSourceFile(AudioBook audioBook, string contentUri)
        {
            AudioBook = audioBook;
            ContentUri = contentUri;
        }
    }
}