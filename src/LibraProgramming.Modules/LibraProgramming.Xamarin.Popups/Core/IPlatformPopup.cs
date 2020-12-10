using LibraProgramming.Xamarin.Popups.Platforms.Xamarin;
using System.Threading.Tasks;

namespace LibraProgramming.Xamarin.Popups.Core
{
    public interface IPlatformPopup
    {
        bool IsInitialized
        {
            get;
        }

        Task AddAsync(PopupContentPage page);

        Task RemoveAsync(PopupContentPage page);
    }
}
