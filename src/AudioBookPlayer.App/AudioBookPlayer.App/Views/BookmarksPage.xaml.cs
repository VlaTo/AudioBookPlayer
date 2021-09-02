using AudioBookPlayer.App.Core.Attributes;
using AudioBookPlayer.App.ViewModels;
using AudioBookPlayer.App.ViewModels.RequestContexts;
using System;
using System.Diagnostics;
using Xamarin.Forms.Xaml;

namespace AudioBookPlayer.App.Views
{
    [ViewModel(typeof(BookmarksViewModel))]
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BookmarksPage : IDisposable
    {
        private readonly BookmarkRequestContext context;
        private readonly Action callback;

        public BookmarksPage(BookmarkRequestContext context, Action callback)
        {
            this.context = context;
            this.callback = callback;

            InitializeComponent();
        }

        protected override bool OnBackButtonPressed()
        {
            var handled = base.OnBackButtonPressed();

            if (handled)
            {
                callback.Invoke();
            }

            return handled;
        }

        public void Dispose()
        {
            Debug.WriteLine("[BookmarksPage] [Dispose] Execute");
        }
    }
}