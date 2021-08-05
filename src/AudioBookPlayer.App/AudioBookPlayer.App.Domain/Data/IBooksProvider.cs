using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AudioBookPlayer.App.Domain.Models;

namespace AudioBookPlayer.App.Domain.Data
{
    public interface IBooksProvider
    {
        Task<IReadOnlyList<AudioBook>> QueryBooksAsync(CancellationToken cancellationToken = default);
    }
}
