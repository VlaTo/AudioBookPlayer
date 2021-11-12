namespace AudioBookPlayer.App.Domain.Models
{
    public sealed class AudioBookPosition
    {
        public MediaId MediaId
        {
            get;
        }

        public long QueueItemId
        {
            get;
        }

        public long MediaPosition
        {
            get;
        }

        public AudioBookPosition(MediaId mediaId, long queueId, long mediaPosition)
        {
            MediaId = mediaId;
            QueueItemId = queueId;
            MediaPosition = mediaPosition;
        }
    }
}