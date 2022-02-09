using Android.Content;
using Android.Content.Res;
using AudioBookPlayer.App.Models;
using System;
using System.Collections.Generic;
using System.Globalization;

#nullable enable

namespace AudioBookPlayer.App.Views
{
    internal sealed class AllBooksListAdapter : BooksListAdapter
    {
        public AllBooksListAdapter(Context context, Resources? resources)
            : base(new Sorter(), context, resources)
        {
        }

        public override void Detach()
        {
            base.Detach();
        }

        private sealed class Sorter : IComparer<BaseItem>
        {
            private readonly StringComparer comparer;

            public Sorter()
            {
                comparer = StringComparer.Create(CultureInfo.CurrentUICulture, CompareOptions.None);
            }

            public int Compare(BaseItem x, BaseItem y)
            {
                if (x is BookItem one)
                {
                    if (one.RecentActionTime.HasValue)
                    {
                        if (y is BookItem two)
                        {
                            if (two.RecentActionTime.HasValue)
                            {
                                return comparer.Compare(one.Title, two.Title);
                            }
                        }
                    }
                    else
                    {
                        return -1;
                    }
                }

                return 1;
            }
        }
    }
}

#nullable restore