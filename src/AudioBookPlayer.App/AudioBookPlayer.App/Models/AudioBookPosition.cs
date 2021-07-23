namespace AudioBookPlayer.App.Models
{
    public sealed class AudioBookPosition
    {
        public string Name
        {
            get;
        }

        public BookPosition BookPosition
        {
            get;
        }

        public AudioBookPosition(BookPosition bookPosition, string name = null)
        {
            BookPosition = bookPosition;
            Name = name;
        }
    }
}