namespace AudioBookPlayer.App.ViewModels
{
    public sealed class ChapterViewModel : ViewModelBase
    {
        private string title;

        public string Title
        {
            get => title;
            set => SetProperty(ref title, value);
        }
    }
}