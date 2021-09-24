using System.Collections.Generic;
using AudioBookPlayer.App.Domain.Models;

namespace AudioBookPlayer.App.Domain.Providers
{
    /// <summary>
    /// 
    /// </summary>
    public interface IBooksProvider
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        IReadOnlyList<AudioBook> QueryBooks();
    }
}
