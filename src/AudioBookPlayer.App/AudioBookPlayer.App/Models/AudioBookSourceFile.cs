namespace AudioBookPlayer.App.Models
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

        public long Length
        {
            get;
        }

        public AudioBookSourceFile(AudioBook audioBook, string contentUri, long length)
        {
            AudioBook = audioBook;
            ContentUri = contentUri;
            Length = length;
        }
    }
}