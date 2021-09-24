namespace AudioBookPlayer.App.Domain.Models
{
    public sealed partial class SectionItem
    {
        public EntityId BookId
        {
            get;
            private set;
        }

        public int Index
        {
            get;
            private set;
        }

        public string Title
        {
            get;
            private set;
        }

        public string ContentUri
        {
            get;
            private set;
        }

        public SectionItem()
        {
        }
    }
}