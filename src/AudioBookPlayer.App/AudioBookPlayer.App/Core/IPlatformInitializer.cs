using LibraProgramming.Xamarin.Dependency.Container;

namespace AudioBookPlayer.App.Core
{
    public interface IPlatformInitializer
    {
        void RegisterTypes(DependencyContainer container);
    }
}
