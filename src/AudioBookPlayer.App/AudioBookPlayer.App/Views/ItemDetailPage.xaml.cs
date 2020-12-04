using AudioBookPlayer.App.ViewModels;
using System.ComponentModel;
using Xamarin.Forms;

namespace AudioBookPlayer.App.Views
{
    public partial class ItemDetailPage : ContentPage
    {
        public ItemDetailPage()
        {
            InitializeComponent();
            BindingContext = new ItemDetailViewModel();
        }
    }
}