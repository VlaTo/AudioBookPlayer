using Android.Content;
using Android.Content.Res;
using AudioBookPlayer.App.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using Android.Views;
using AndroidX.Fragment.App;
using AudioBookPlayer.App.Views.Fragments;

#nullable enable

namespace AudioBookPlayer.App.Views
{
    internal sealed class AllBooksListAdapter : BooksListAdapter
    {
        private readonly FragmentManager fragmentManager;

        public AllBooksListAdapter(
            Context context,
            Resources? resources,
            FragmentManager fragmentManager)
            : base(new Sorter(), context, resources)
        {
            this.fragmentManager = fragmentManager;
        }

        public override void Detach()
        {
            base.Detach();
        }

        protected override void OnBookMoreAction(View? view)
        {
            var dialog = SectionReorderDialogFragment.NewInstance();

            dialog.Show(fragmentManager, null);
        }

        #region Sorter

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

        #endregion
    }
}

#nullable restore