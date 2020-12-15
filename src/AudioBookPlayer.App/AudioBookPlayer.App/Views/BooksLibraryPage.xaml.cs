using AudioBookPlayer.App.Core.Attributes;
using AudioBookPlayer.App.ViewModels;
using LibraProgramming.Xamarin.Interaction;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AudioBookPlayer.App.Views
{
    public sealed class DummyTriggerAction : TriggerAction<BindableObject>
    {
        protected override void Invoke(BindableObject sender)
        {
            ;
        }
    }

    [ViewModel(typeof(BooksLibraryViewModel))]
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BooksLibraryPage : ContentPage
    {
        public BooksLibraryPage()
        {
            InitializeComponent();
        }

        private async void OnSelectSourceFolder()
        {
            var page = new ChooseLibraryFolderPopup();

            await Shell.Current.Navigation.PushModalAsync(page);

        }

        private void OnSelectFolderRequest(InteractionRequestContext context, Action callback)
        {

            //var page = (BooksLibraryPage)sender;
            //var temp1 = page.GetValue(Interaction.RequestTriggersProperty);
            //var triggers = Interaction.GetRequestTriggers(page);

            //foreach(var trigger in triggers)
            //{
            //    trigger.BindingContext = this.BindingContext;


            //}
        }
    }
}