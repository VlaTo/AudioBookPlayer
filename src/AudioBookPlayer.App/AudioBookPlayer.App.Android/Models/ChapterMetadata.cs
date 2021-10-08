using Android.Support.V4.Media.Session;
using AudioBookPlayer.App.Models;

namespace AudioBookPlayer.App.Android.Models
{
    /// <summary>
    /// 
    /// </summary>
    internal sealed class ChapterMetadata : IChapterMetadata
    {
        private readonly MediaSessionCompat.QueueItem queueItem;

        public long QueueId => queueItem.QueueId;

        public string MediaId => queueItem.Description.MediaId;

        public string Title => queueItem.Description.Title;

        public ISectionMetadata Section
        {
            get;
        }

        public ChapterMetadata(MediaSessionCompat.QueueItem queueItem)
        {
            this.queueItem = queueItem;
            Section = new SectionMetadata(queueItem.Description.Extras);
        }
    }
}