using System.Threading.Tasks;
using LibraProgramming.Xamarin.Popups.Views;

namespace LibraProgramming.Xamarin.Popups.Core
{
    /// <summary>
    /// 
    /// </summary>
    public interface IPlatformPopup
    {
        /// <summary>
        /// 
        /// </summary>
        bool IsInitialized
        {
            get;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        Task AddAsync(PopupContentPage page);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        Task RemoveAsync(PopupContentPage page);
    }
}
