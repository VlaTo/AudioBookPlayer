namespace AudioBookPlayer.App.ViewModels
{
    public class ChapterViewModel : ViewModelBase, IChapterViewModel
    {
        private string title;

        public int Index
        {
            get;
        }

        public long QueueId
        {
            get;
        }

        public string Title
        {
            get => title;
            set => SetProperty(ref title, value);
        }

        public ChapterViewModel(int index, long queueId)
        {
            Index = index;
            QueueId = queueId;
        }
    }
}