using AudioBookPlayer.App.Core.Attributes;
using AudioBookPlayer.App.ViewModels;
using AudioBookPlayer.App.ViewModels.RequestContexts;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AudioBookPlayer.App.Views
{
    [ViewModel(typeof(ChapterPickerViewModel))]
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ChapterPickerPopup
    {
        private readonly PickChapterRequestContext pickChapterContext;
        private readonly Action pickChapterCallback;

        public ChapterPickerPopup(PickChapterRequestContext pickChapterContext, Action pickChapterCallback)
        {
            this.pickChapterContext = pickChapterContext;
            this.pickChapterCallback = pickChapterCallback;

            InitializeComponent();
            
            if (BindingContext is ChapterPickerViewModel)
            {
                ;
            }
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            if (null != pickChapterCallback)
            {
                pickChapterCallback.Invoke();
            }
        }

        private async void OnCloseRequest(object sender, ClosePopupRequestContext context, Action _)
        {
            await Shell.Current.Navigation.PopModalAsync(context.Animated);
        }
    }
}