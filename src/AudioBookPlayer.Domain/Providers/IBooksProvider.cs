using System.Collections.Generic;
using AudioBookPlayer.Domain.Models;

namespace AudioBookPlayer.Domain.Providers
{
    public interface IBooksProvider
    {
        IReadOnlyList<AudioBook> QueryBooks();
    }
}