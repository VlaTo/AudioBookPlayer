using System.Threading.Tasks;
using LibraProgramming.Xamarin.Popups.Views;

namespace LibraProgramming.Xamarin.Popups.Services
{
    /// <summary>
    /// 
    /// </summary>
    public interface IPopupService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        Task ShowPopupAsync(PopupContentPage page);
    }
}
