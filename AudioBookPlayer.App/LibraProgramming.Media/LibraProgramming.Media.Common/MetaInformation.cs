using System;
using System.Collections.Generic;

namespace LibraProgramming.Media.Common
{
    public sealed class MetaInformation
    {
        private readonly List<MetaInformationItem> items;

        public IReadOnlyList<MetaInformationItem> Items => items;

        public MetaInformation()
        {
            items = new List<MetaInformationItem>();
        }

        public void Add(MetaInformationItem item)
        {
            if (null == item)
            {
                throw new ArgumentNullException(nameof(item));
            }

            items.Add(item);
        }
    }
}
