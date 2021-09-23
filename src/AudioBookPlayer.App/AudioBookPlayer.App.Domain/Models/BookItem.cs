using System;

namespace AudioBookPlayer.App.Domain.Models
{
    public sealed partial class BookItem
    {
        public EntityId Id
        {
            get;
        }

        public string Title
        {
            get;
        }

        public TimeSpan Duration
        {
            get;
            private set;
        }

        public TimeSpan Position
        {
            get;
            private set;
        }

        public bool IsCompleted
        {
            get;
            private set;
        }

        public string[] Authors
        {
            get;
            private set;
        }

        public string[] Covers
        {
            get; 
            private set;
        }

        private BookItem(EntityId id, string title)
        {
            Id = id;
            Title = title;
        }
    }
}