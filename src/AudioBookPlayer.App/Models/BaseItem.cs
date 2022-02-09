namespace AudioBookPlayer.App.Models
{
    public class BaseItem : Java.Lang.Object
    {
        public string Title
        {
            get;
        }

        public BaseItem(string title)
        {
            Title = title;
        }
    }
}