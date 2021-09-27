using AudioBookPlayer.App.Domain.Models;
using LibraProgramming.Xamarin.Core;

namespace AudioBookPlayer.App.Android.Services
{
    internal sealed class InMemoryBookItemsCache : InMemoryCache<string, BookItem>, IBookItemsCache
    {
    }
}