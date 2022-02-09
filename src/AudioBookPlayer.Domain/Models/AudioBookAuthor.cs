namespace AudioBookPlayer.Domain.Models
{
    public sealed class AudioBookAuthor
    {
        public AudioBook AudioBook
        {
            get;
        }

        public string Name
        {
            get;
        }

        public AudioBookAuthor(AudioBook audioBook, string name)
        {
            AudioBook = audioBook;
            Name = name;
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }
    }
}