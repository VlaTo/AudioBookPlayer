using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AudioBookPlayer.App.Domain.Models;

namespace AudioBookPlayer.App.Domain.Providers
{
    public interface IBooksProvider
    {
        Task<IReadOnlyList<AudioBook>> QueryBooksAsync(CancellationToken cancellationToken = default);

        IReadOnlyList<AudioBook> QueryBooks();
    }
}
