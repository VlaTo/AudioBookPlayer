using AudioBookPlayer.App.Domain.Models;
using LibraProgramming.Xamarin.Core;

namespace AudioBookPlayer.App.Android.Services
{
    public interface IBookItemsCache : ICache<string, BookItem>
    {
    }
}