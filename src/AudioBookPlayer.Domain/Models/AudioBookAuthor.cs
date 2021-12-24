namespace AudioBookPlayer.Domain.Models
{
    public sealed class AudioBookAuthor
    {
        public string Name
        {
            get;
        }

        public AudioBookAuthor(string name)
        {
            Name = name;
        }
    }
}