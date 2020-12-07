using AudioBookPlayer.App.Models;
using System.Collections.Generic;

namespace AudioBookPlayer.App.Services
{
    public interface IBookShelfProvider
    {
        IReadOnlyCollection<AudioBook> GetBooks();
    }
}
