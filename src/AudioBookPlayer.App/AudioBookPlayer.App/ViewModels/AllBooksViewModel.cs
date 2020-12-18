using LibraProgramming.Xamarin.Interaction.Contracts;

namespace AudioBookPlayer.App.ViewModels
{
    public sealed class AllBooksViewModel : ViewModelBase, IInitialize
    {
        void IInitialize.OnInitialize()
        {
            System.Diagnostics.Debug.WriteLine($"[AllBooksViewModel] [OnInitialize] Executed");
        }
    }
}
