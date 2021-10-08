namespace AudioBookPlayer.App.Models
{
    public interface ISectionMetadata
    {
        string Name
        {
            get;
        }

        int Index
        {
            get;
        }

        string ContentUri
        {
            get;
        }
    }
}