namespace AudioBookPlayer.App.Models
{
    public interface IChapterMetadata
    {
        long QueueId
        {
            get;
        }

        string MediaId
        {
            get;
        }

        string Title
        {
            get;
        }

        ISectionMetadata Section
        {
            get;
        }
    }
}