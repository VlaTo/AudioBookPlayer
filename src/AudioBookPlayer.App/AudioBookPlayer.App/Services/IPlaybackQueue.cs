namespace AudioBookPlayer.App.Services
{
    public interface IPlaybackQueue
    {
        /// <summary>
        /// 
        /// </summary>
        long ActiveQueueItemId { get; }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="queueItemId"></param>
        void SetActiveQueueItemId(long queueItemId);
    }
}