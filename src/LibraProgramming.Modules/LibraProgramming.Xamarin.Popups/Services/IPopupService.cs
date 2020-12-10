using LibraProgramming.Xamarin.Popups.Platforms.Xamarin;
using System.Threading.Tasks;

namespace LibraProgramming.Xamarin.Popups.Services
{
    public interface IPopupService
    {
        Task ShowPopupAsync(PopupContentPage page);
    }
}
