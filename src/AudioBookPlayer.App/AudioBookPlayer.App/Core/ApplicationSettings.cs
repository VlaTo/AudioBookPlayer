using Xamarin.Essentials;

namespace AudioBookPlayer.App.Core
{
    public sealed class ApplicationSettings
    {
        private const string LibraryRootPathKey = "Library.RootPath";

        public string LibraryRootPath
        {
            get
            {
                var value = Preferences.Get(
                    LibraryRootPathKey,
                    DefaultSettings.Current.LibraryRootPath
                );
                return value;
            }
            set
            {
                Preferences.Set(
                    LibraryRootPathKey,
                    value
                );
            }
        }

        public ApplicationSettings()
        {

        }
    }
}
