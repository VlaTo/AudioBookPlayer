namespace AudioBookPlayer.App.Models
{
    public interface IAudioBookMetadata
    {
        string Title
        {
            get;
        }

        string Subtitle
        {
            get;
        }

        string Description
        {
            get;
        }

        string AlbumArtUri
        {
            get;
        }
    }
}