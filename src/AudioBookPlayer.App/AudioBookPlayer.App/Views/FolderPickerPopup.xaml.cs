using AudioBookPlayer.App.Core.Attributes;
using AudioBookPlayer.App.ViewModels;
using LibraProgramming.Xamarin.Interaction.Contracts;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AudioBookPlayer.App.Views
{
    [ViewModel(typeof(FolderPickerViewModel))]
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class FolderPickerPopup : ContentPage, IInitialize
    {
        public FolderPickerPopup()
        {
            InitializeComponent();
        }

        public Task InitializePathAsync(string path)
        {
            if (BindingContext is FolderPickerViewModel viewModel && null != viewModel)
            {
                return viewModel.InitializePathAsync(path);
            }

            return Task.CompletedTask;
        }

        void IInitialize.OnInitialize()
        {
            ;
        }

        private async void OnApplyRequest(object sender, CloseInteractionRequestContext context)
        {
            await Shell.Current.Navigation.PopModalAsync();
        }
    }
}