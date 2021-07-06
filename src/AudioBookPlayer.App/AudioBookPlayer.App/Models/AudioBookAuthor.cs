namespace AudioBookPlayer.App.Models
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