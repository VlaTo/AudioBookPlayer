namespace AudioBookPlayer.App.ViewModels
{
    public sealed class BookmarkViewModel : ViewModelBase
    {
        private string title;

        public string Title
        {
            get => title;
            set => SetProperty(ref title, value);
        }
    }
}