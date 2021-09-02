using AudioBookPlayer.App.Core.Attributes;
using AudioBookPlayer.App.ViewModels;
using LibraProgramming.Xamarin.Core;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AudioBookPlayer.App.Views
{
    [ViewModel(typeof(FolderPickerViewModel))]
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class FolderPickerPopup : ContentPage
    {
        private readonly Deferral<string> deferral;

        public FolderPickerPopup(SourceFolderRequestContext context)
        {
            deferral = context.GetDeferral();

            InitializeComponent();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            if (false == deferral.IsCompleted)
            {
                deferral.Cancel();
            }
        }

        private async void OnCloseRequest(object _, CloseInteractionRequestContext requestContext)
        {
            deferral.Complete(requestContext.Argument);
            await Shell.Current.Navigation.PopModalAsync();
        }
    }
}