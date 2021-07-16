using AudioBookPlayer.App.Core.Attributes;
using AudioBookPlayer.App.ViewModels;
using System;
using Xamarin.Forms.Xaml;

namespace AudioBookPlayer.App.Views
{
    [ViewModel(typeof(ChapterPickerViewModel))]
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ChapterPickerPopup
    {
        private readonly PickChapterRequestContext context;
        private readonly Action callback;

        public ChapterPickerPopup(PickChapterRequestContext context, Action callback)
        {
            this.context = context;
            this.callback = callback;

            InitializeComponent();
            
            if (BindingContext is ChapterPickerViewModel)
            {
                ;
            }
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            if (null != callback)
            {
                callback.Invoke();
            }
        }
    }
}