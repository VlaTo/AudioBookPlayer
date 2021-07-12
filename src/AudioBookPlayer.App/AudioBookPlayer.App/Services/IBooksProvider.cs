using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AudioBookPlayer.App.Models;

namespace AudioBookPlayer.App.Services
{
    public interface IBooksProvider
    {
        Task<IReadOnlyList<AudioBook>> QueryBooksAsync(CancellationToken cancellationToken = default);
    }
}
