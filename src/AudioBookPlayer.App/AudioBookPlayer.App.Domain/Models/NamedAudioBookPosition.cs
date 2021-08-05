using AudioBookPlayer.App.Domain.Data;

namespace AudioBookPlayer.App.Domain.Models
{
    public sealed class NamedAudioBookPosition : IEntity
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