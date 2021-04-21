using AudioBookPlayer.App.Services;
using LibraProgramming.Xamarin.Interaction.Contracts;

namespace AudioBookPlayer.App.ViewModels
{
    public sealed class AllBooksViewModel : ViewModelBase, IInitialize
    {
        private readonly IBookShelfProvider provider;

        public AllBooksViewModel(IBookShelfProvider provider)
        {
            this.provider = provider;
        }

        void IInitialize.OnInitialize()
        {
            var books = provider.GetBooks();

            System.Diagnostics.Debug.WriteLine($"[AllBooksViewModel] [OnInitialize] Executed");
        }
    }
}
