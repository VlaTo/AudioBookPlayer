using Xamarin.Forms;

namespace LibraProgramming.Xamarin.Interaction.Contracts
{
    public interface ILifetimeTarget
    {
        void OnAppearing(Page page);

        void OnDisappearing(Page page);
    }
}
