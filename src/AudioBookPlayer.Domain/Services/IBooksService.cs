using AudioBookPlayer.Domain.Models;
using AudioBookPlayer.Domain.Providers;

namespace AudioBookPlayer.Domain.Services
{
    public interface IBooksService : IBooksProvider
    {
        void SaveBook(AudioBook audioBook);
    }
}