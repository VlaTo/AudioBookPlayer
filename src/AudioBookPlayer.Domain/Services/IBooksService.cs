using AudioBookPlayer.Domain.Models;
using AudioBookPlayer.Domain.Providers;

namespace AudioBookPlayer.Domain.Services
{
    public interface IBooksService : IBooksProvider
    {
        void SaveBook(AudioBook audioBook);

        bool UpdateBook(long id, AudioBook audioBook);

        bool RemoveBook(AudioBook audioBook);
    }
}