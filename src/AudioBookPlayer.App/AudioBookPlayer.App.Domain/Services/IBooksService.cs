using AudioBookPlayer.App.Domain.Models;
using AudioBookPlayer.App.Domain.Providers;

namespace AudioBookPlayer.App.Domain.Services
{
    public interface IBooksService : IBooksProvider
    {
        bool IsEmpty();

        void SaveBook(AudioBook audioBook);

        AudioBook GetBook(EntityId bookId);

        void RemoveBook(AudioBook audioBook);
    }
}