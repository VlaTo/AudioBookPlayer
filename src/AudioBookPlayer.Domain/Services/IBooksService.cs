using AudioBookPlayer.Domain.Models;
using AudioBookPlayer.Domain.Providers;
using System.Collections.Generic;

namespace AudioBookPlayer.Domain.Services
{
    public interface IBooksService : IBooksProvider
    {
        AudioBook GetBook(long bookId);

        void SaveBook(AudioBook audioBook);

        bool UpdateBook(long bookId, AudioBook audioBook);

        bool RemoveBook(AudioBook audioBook);

        IReadOnlyList<HistoryItem> QueryHistory(long bookId);
    }
}