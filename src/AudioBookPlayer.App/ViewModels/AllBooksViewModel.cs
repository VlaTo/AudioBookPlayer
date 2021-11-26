#nullable enable

using System.Collections.Generic;
using AudioBookPlayer.Domain;

namespace AudioBookPlayer.App.ViewModels
{
    internal sealed class AllBooksViewModel
    {
        private static AllBooksViewModel? instance;

        public IReadOnlyList<AudioBookDescription>? BookItems
        {
            get;
            private set;
        }

        public bool HasBookItems => null != BookItems;

        private AllBooksViewModel()
        {
            BookItems = null;
        }

        public static AllBooksViewModel Instance()
        {
            if (null == instance)
            {
                instance = new AllBooksViewModel();
            }

            return instance;
        }

        public void SetBookItems(IReadOnlyList<AudioBookDescription> value)
        {
            BookItems = value;
        }
    }
}