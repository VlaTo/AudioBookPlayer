using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AudioBookPlayer.App.Domain.Models;

namespace AudioBookPlayer.App.Domain.Data
{
    public interface IBookmarkRepository : IRepository<NamedAudioBookPosition>
    {
        Task<IEnumerable<NamedAudioBookPosition>> QueryAsync(long bookId, CancellationToken cancellationToken = default);
    }
}