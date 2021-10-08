namespace AudioBookPlayer.App.ViewModels
{
    public sealed class SectionViewModel : GroupEntry<ChapterViewModel>, IChapterViewModel
    {
        private int index;

        public int Index
        {
            get => index;
            set => SetProperty(ref index, value);
        }
    }
}