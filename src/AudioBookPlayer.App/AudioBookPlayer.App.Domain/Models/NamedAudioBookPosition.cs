namespace AudioBookPlayer.App.Domain.Models
{
    public sealed class NamedAudioBookPosition
    {
        public string Name
        {
            get;
        }

        public AudioBookPosition AudioBookPosition
        {
            get;
        }

        public NamedAudioBookPosition(AudioBookPosition audioBookPosition, string name = null)
        {
            AudioBookPosition = audioBookPosition;
            Name = name;
        }
    }
}