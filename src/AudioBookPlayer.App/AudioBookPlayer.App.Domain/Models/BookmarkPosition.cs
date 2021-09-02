using AudioBookPlayer.App.Domain.Data;

namespace AudioBookPlayer.App.Domain.Models
{
    public sealed class BookmarkPosition : IEntity
    {
        public string Name
        {
            get;
        }

        public AudioBookPosition AudioBookPosition
        {
            get;
        }

        public BookmarkPosition(AudioBookPosition audioBookPosition, string name = null)
        {
            AudioBookPosition = audioBookPosition;
            Name = name;
        }
    }
}