namespace AudioBookPlayer.App.ViewModels
{
    public class ChapterViewModel : ViewModelBase, IChapterViewModel
    {
        private readonly int index;
        private string title;

        public int Index => index;

        public string Title
        {
            get => title;
            set => SetProperty(ref title, value);
        }

        public ChapterViewModel(int index)
        {
            this.index = index;
        }
    }
}