namespace LibraProgramming.Xamarin.Interaction.Contracts
{
    public interface IPageLifecycleAware
    {
        void OnAppearing();
        
        void OnDisappearing();
    }
}
